// Decompiled with JetBrains decompiler
// Type: MPOST.IStackedEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("CE771CF8-D92C-43E2-85E1-EE89306D6D49")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IStackedEventArgs
  {
    DocumentType DocType { [DispId(1)] get; }

    IDocument Document { [DispId(2)] get; }
  }
}
