fn main() {
    csbindgen::Builder::default()
        .input_extern_file("src/ffi.rs")
        .csharp_dll_name("ai_engine")
        .csharp_namespace("AiEngine.Native.Interop")
        .csharp_class_name("NativeMethods")
        .csharp_class_accessibility("internal")
        .generate_csharp_file("../AiEngine.Native/NativeMethods.g.cs")
        .unwrap();
}
