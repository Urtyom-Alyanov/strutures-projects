using TfIdf.Matrix;

namespace TfIdf;

/// <summary>
/// Векторизатор TF/IDF
/// </summary>
public class TfIdfVectorizer
{
    public Dictionary<string, uint> Vocabulary { get; private set; } = new();
    public double[] Idf { get; private set; } = Array.Empty<double>();

    /// <summary>
    /// Обучает векторизатор на документах
    /// </summary>
    /// <param name="documents">Документы</param>
    /// <returns>TF/IDF матрица</returns>
    public SparseMatrix FitTransform(List<string> documents)
    {
        uint numDocs = (uint)documents.Count;
        
        var docTokensList = new List<List<string>>();
        var vocabSet = new HashSet<string>();
        
        foreach (var tokens in documents.Select(TextProcessor.TokenizeAndNormalize))
        {
            docTokensList.Add(tokens);
            foreach (var token in tokens)
            {
                vocabSet.Add(token);
            }
        }
        
        var vocabularyList = vocabSet.OrderBy(v => v).ToList();
        Vocabulary.Clear();
        for (uint i = 0; i < vocabularyList.Count; i++)
        {
            Vocabulary[vocabularyList[(int)i]] = i;
        }

        uint vocabSize = (uint)Vocabulary.Count;
        
        var df = new int[vocabSize];
        foreach (var tokens in docTokensList)
        {
            var uniqueTokensInDoc = new HashSet<string>(tokens);
            foreach (var token in uniqueTokensInDoc)
            {
                if (Vocabulary.TryGetValue(token, out uint colIdx))
                {
                    df[colIdx]++;
                }
            }
        }
        
        Idf = new double[vocabSize];
        for (int j = 0; j < vocabSize; j++)
        {
            Idf[j] = Math.Log((1.0 + numDocs) / (1.0 + df[j])) + 1.0;
        }
        
        var values = new List<double>();
        var colIndices = new List<uint>();
        var rowPtr = new uint[numDocs + 1];

        for (int i = 0; i < numDocs; i++)
        {
            var tokens = docTokensList[i];
            if (tokens.Count == 0)
            {
                rowPtr[i + 1] = (uint)values.Count;
                continue;
            }

            var termCounts = new Dictionary<uint, uint>();
            foreach (var token in tokens)
            {
                if (!Vocabulary.TryGetValue(token, out uint colIdx))
                    continue;

                termCounts.TryAdd(colIdx, 0);
                termCounts[colIdx]++;
            }

            var sortedCols = termCounts.Keys.OrderBy(c => c).ToList();

            foreach (uint colIdx in sortedCols)
            {
                double rawCount = termCounts[colIdx];
                double tf = rawCount / tokens.Count;
                double tfidf = tf * Idf[colIdx];

                values.Add(tfidf);
                colIndices.Add(colIdx);
            }

            rowPtr[i + 1] = (uint)values.Count;
        }

        return new(values.ToArray(), colIndices.ToArray(), rowPtr, numDocs, vocabSize);
    }
}