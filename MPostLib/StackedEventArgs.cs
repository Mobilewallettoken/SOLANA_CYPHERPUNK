// Decompiled with JetBrains decompiler
// Type: MPOST.StackedEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("A55F45B3-DAF0-4A7B-BC40-C751EE270DDF")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class StackedEventArgs : EventArgs, IStackedEventArgs
  {
    public readonly DocumentType docType;
    public readonly IDocument document;

    public StackedEventArgs(DocumentType docType, IDocument document)
    {
      this.docType = docType;
      this.document = document;
    }

    public DocumentType DocType => this.docType;

    public IDocument Document => this.document;
  }
}
