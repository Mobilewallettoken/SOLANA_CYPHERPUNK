// Decompiled with JetBrains decompiler
// Type: MPOST.Coupon
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("0248DE5A-341C-440a-92BD-D89F633C875F")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class Coupon : ICoupon_API, IDocument
  {
    private int ownerID;
    private double value;

    public Coupon(int ownerID, double value)
    {
      this.ownerID = ownerID;
      this.value = value;
    }

    public int OwnerID => this.ownerID;

    public double Value => this.value;

    public override string ToString() => string.Format("Value: {0}, OwnerID: {1}", (object) this.Value.ToString(), (object) this.OwnerID.ToString());

    public string ValueString => string.Format("Value: {0}, OwnerID: {1}", (object) this.Value.ToString(), (object) this.OwnerID.ToString());
  }
}
