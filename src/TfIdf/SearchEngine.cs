using TfIdf.Matrix;

namespace TfIdf;

/// <summary>
/// Векторизатор TF/IDF
/// </summary>
public class SearchEngine
{
    private Dictionary<string, uint> _vocabulary = new();
    private double[] _idf = Array.Empty<double>();
    
    public SparseMatrix DocumentMatrix { get; private set; }

    /// <summary>
    /// Обучает векторизатор на документах
    /// </summary>
    /// <param name="documents">Документы</param>
    /// <returns>TF/IDF матрица</returns>
    public void FitTransform(List<string> documents)
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
        _vocabulary.Clear();
        for (uint i = 0; i < vocabularyList.Count; i++)
        {
            _vocabulary[vocabularyList[(int)i]] = i;
        }

        uint vocabSize = (uint)_vocabulary.Count;
        
        var df = new int[vocabSize];
        foreach (var tokens in docTokensList)
        {
            var uniqueTokensInDoc = new HashSet<string>(tokens);
            foreach (var token in uniqueTokensInDoc)
            {
                if (_vocabulary.TryGetValue(token, out uint colIdx))
                {
                    df[colIdx]++;
                }
            }
        }
        
        _idf = new double[vocabSize];
        for (int j = 0; j < vocabSize; j++)
        {
            _idf[j] = Math.Log((1.0 + numDocs) / (1.0 + df[j])) + 1.0;
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
                if (!_vocabulary.TryGetValue(token, out uint colIdx))
                    continue;

                termCounts.TryAdd(colIdx, 0);
                termCounts[colIdx]++;
            }

            var sortedCols = termCounts.Keys.OrderBy(c => c).ToList();

            foreach (uint colIdx in sortedCols)
            {
                double rawCount = termCounts[colIdx];
                double tf = rawCount / tokens.Count;
                double tfidf = tf * _idf[colIdx];

                values.Add(tfidf);
                colIndices.Add(colIdx);
            }

            rowPtr[i + 1] = (uint)values.Count;
        }

        DocumentMatrix = new SparseMatrix(values.ToArray(), colIndices.ToArray(), rowPtr, numDocs, vocabSize);
    }
    
    /// <summary>
    /// Ранжирование документов по запросу
    /// </summary>
    /// <param name="rawQuery">запрос</param>
    /// <returns>Индексы и релевантность</returns>
    public List<(int DocIndex, double Score)> RankDocuments(string rawQuery)
    {
        var queryTokens = TextProcessor.TokenizeAndNormalize(rawQuery);
        var queryVector = new double[_vocabulary.Count];

        foreach (var token in queryTokens)
        {
            if (_vocabulary.TryGetValue(token, out uint col))
            {
                queryVector[col] += 1.0 / queryTokens.Count;
            }
        }

        double[] scores = DocumentMatrix.Multiply(queryVector);

        var ranked = scores.Select((score, index) => (DocIndex: index, Score: score))
            .OrderByDescending(x => x.Score)
            .ToList();

        return ranked;
    }
}