// Decompiled with JetBrains decompiler
// Type: MPOST.BNFErrorStatus
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("12F50A05-9C0F-4A87-940B-0E6B61CC9042")]
  [ComVisible(true)]
  public enum BNFErrorStatus
  {
    NoError,
    MotorStall,
    CartridgeRemoved,
    StubOut,
    CoveredStubOut,
    TooManyRejects,
  }
}
