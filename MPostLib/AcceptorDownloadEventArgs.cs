// Decompiled with JetBrains decompiler
// Type: MPOST.AcceptorDownloadEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("C598B1E0-946C-4861-8EE6-3ED65A4F1F26")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class AcceptorDownloadEventArgs : EventArgs, IDownloadEventArgs
  {
    private int sectorCount;

    public AcceptorDownloadEventArgs(int sectorCount) => this.sectorCount = sectorCount;

    public int SectorCount => this.sectorCount;
  }
}
