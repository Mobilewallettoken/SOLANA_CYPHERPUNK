// Decompiled with JetBrains decompiler
// Type: MPOST.AuditContainer
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Text;

namespace MobileWallet.Desktop.MPostLib
{
  public class AuditContainer
  {
    internal const int INT_MAX_ORIENTATIONS = 4;
    private const int INT_MAX_FIELDS = 3;
    public const int INT_RIGHT_UP = 0;
    public const int INT_RIGHT_DOWN = 1;
    public const int INT_LEFT_UP = 2;
    public const int INT_LEFT_DOWN = 3;
    internal const int INT_FIELD_RECOGNIZED = 23;
    internal const int INT_FIELD_VALIDATED = 24;
    internal const int INT_FIELD_STACKED = 25;
    internal const int INT_FIELD_MAX_VALUE = 26;
    private List<int[]> _lstRecognized;
    private List<int[]> _lstValidated;
    private List<int[]> _lstStacked;
    private int _iMaxNotes;
    private int _iFieldsPerQuery;
    private int _iNumDatasets;
    private int _iDataWidth;

    public int MaximumNoteCount => this._iMaxNotes;

    internal int NumberOfDatasets => this._iNumDatasets;

    internal int DataWidth => this._iDataWidth;

    internal AuditContainer(int maxNotes, int numFieldsPerQuery, int dataWidth)
    {
      this._iMaxNotes = maxNotes;
      if (this._iMaxNotes == (int) sbyte.MaxValue)
        this._iMaxNotes = 128;
      this._iFieldsPerQuery = numFieldsPerQuery;
      this._iNumDatasets = this._iMaxNotes / numFieldsPerQuery;
      if (this._iMaxNotes % numFieldsPerQuery != 0)
        ++this._iNumDatasets;
      this._iDataWidth = dataWidth;
      this._lstRecognized = new List<int[]>();
      for (int index = 0; index < 4; ++index)
        this._lstRecognized.Add(new int[this._iMaxNotes]);
      this._lstValidated = new List<int[]>();
      for (int index = 0; index < 4; ++index)
        this._lstValidated.Add(new int[this._iMaxNotes]);
      this._lstStacked = new List<int[]>();
      for (int index = 0; index < 4; ++index)
        this._lstStacked.Add(new int[this._iMaxNotes]);
    }

    internal void appendResults(int field, int orientation, int dataset, byte[] data)
    {
      List<int> intList = new List<int>();
      int num1 = 0;
      if (orientation < 0 || orientation >= 4)
        throw new ArgumentOutOfRangeException("orienation", string.Format("'Orientation' must be a positive value less than {0}. Detected {1}", (object) 4, (object) orientation));
      int[] destinationArray;
      switch (field)
      {
        case 23:
          destinationArray = this._lstRecognized[orientation];
          break;
        case 24:
          destinationArray = this._lstValidated[orientation];
          break;
        case 25:
          destinationArray = this._lstStacked[orientation];
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (field), string.Format("'Field' must be a positive value less than {0}. Detected {1}", (object) 3, (object) field));
      }
      if (data.Length % this._iDataWidth != 0)
        throw new Exception(string.Format("Unexpected number of data bytes. Length must be divisible by {0} but detected a length of {1}", (object) this._iDataWidth, (object) data.Length));
      for (int index = 0; index < data.Length; ++index)
      {
        if (index != 0 && index % this._iDataWidth == 0)
        {
          intList.Add(num1);
          num1 = 0;
        }
        int num2 = 4 * (this._iDataWidth - 1 - index % this._iDataWidth);
        num1 |= ((int) data[index] & 15) << num2;
      }
      intList.Add(num1);
      int destinationIndex = dataset * this._iFieldsPerQuery;
      if (destinationIndex + intList.Count > destinationArray.Length)
        throw new Exception(string.Format("Inserting the data will result in an overflow. Data requires 0x{0:X2} bytes but only 0x{1:X2} locations remain", (object) intList.Count, (object) (destinationArray.Length - destinationIndex)));
      Array.Copy((Array) intList.ToArray(), 0, (Array) destinationArray, destinationIndex, intList.Count);
    }

    public int[] getRecognizedData(Orientation orientation) => orientation < Orientation.Unknown ? this.getRecognizedData((int) orientation) : throw new ArgumentOutOfRangeException("orienation", string.Format("Unable to fetch recognized data for orientation '{0}'", (object) orientation.ToString()));

    public int[] getRecognizedData(int orientationValue) => orientationValue >= 0 && orientationValue < 4 ? this._lstRecognized[orientationValue] : throw new ArgumentOutOfRangeException(nameof (orientationValue), string.Format("'orientationValue' must be a positive value less than {0}. Detected {1}", (object) 4, (object) orientationValue));

    public int[] getValidatedData(Orientation orientation) => orientation < Orientation.Unknown ? this.getValidatedData((int) orientation) : throw new ArgumentOutOfRangeException("orienation", string.Format("Unable to fetch recognized data for orientation '{0}'", (object) orientation.ToString()));

    public int[] getValidatedData(int orientationValue) => orientationValue >= 0 && orientationValue < 4 ? this._lstValidated[orientationValue] : throw new ArgumentOutOfRangeException(nameof (orientationValue), string.Format("'orientationValue' must be a positive value less than {0}. Detected {1}", (object) 4, (object) orientationValue));

    public int[] getStackedData(Orientation orientation) => orientation < Orientation.Unknown ? this.getStackedData((int) orientation) : throw new ArgumentOutOfRangeException("orienation", string.Format("Unable to fetch recognized data for orientation '{0}'", (object) orientation.ToString()));

    public int[] getStackedData(int orientationValue) => orientationValue >= 0 && orientationValue < 4 ? this._lstStacked[orientationValue] : throw new ArgumentOutOfRangeException(nameof (orientationValue), string.Format("'orientationValue' must be a positive value less than {0}. Detected {1}", (object) 4, (object) orientationValue));

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format("Number of documents supported: {0}", (object) this._iMaxNotes));
      foreach (int num in this._lstRecognized[0])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstRecognized[1])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstRecognized[2])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstRecognized[3])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstValidated[0])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstValidated[1])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstValidated[2])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstValidated[3])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstStacked[0])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstStacked[1])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstStacked[2])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      foreach (int num in this._lstStacked[3])
        stringBuilder.Append(string.Format("{0} ", (object) num));
      stringBuilder.AppendLine();
      return stringBuilder.ToString();
    }
  }
}
