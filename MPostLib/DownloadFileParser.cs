// Decompiled with JetBrains decompiler
// Type: MPOST.DownloadFileParser
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.IO;

namespace MobileWallet.Desktop.MPostLib
{
  internal class DownloadFileParser
  {
    private List<byte[]> _lstDataPackets = new List<byte[]>();

    public int DataSize { get; private set; }

    public bool Use8BitProtocol { get; private set; }

    public int FinalPacketNumber { get; private set; }

    public DownloadFileParser(string filename, bool use8BitProtocol)
    {
      this.Use8BitProtocol = use8BitProtocol;
      this.DataSize = this.Use8BitProtocol ? 64 : 32;
      this.ScanFile(filename);
    }

    public byte[] GetData(int packetNumber) => this._lstDataPackets[packetNumber];

    private void ScanFile(string filename)
    {
      int num1 = 0;
      using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
      {
        this.FinalPacketNumber = (int) (fileStream.Length / (long) this.DataSize);
        if (fileStream.Length % (long) this.DataSize > 0L)
          ++this.FinalPacketNumber;
        while (true)
        {
          byte[] array = new byte[(this.Use8BitProtocol ? this.DataSize + 2 : this.DataSize * 2 + 4) + 1];
          array[0] = (byte) 80;
          if (this.Use8BitProtocol)
          {
            array[1] = (byte) (num1 & (int) byte.MaxValue);
            array[2] = (byte) ((num1 & 65280) >> 8);
          }
          else
          {
            array[1] = (byte) ((num1 & 61440) >> 12);
            array[2] = (byte) ((num1 & 3840) >> 8);
            array[3] = (byte) ((num1 & 240) >> 4);
            array[4] = (byte) (num1 & 15);
          }
          ++num1;
          for (int index = 0; index < this.DataSize; ++index)
          {
            int num2 = fileStream.ReadByte();
            if (num2 == -1)
            {
              if (this.Use8BitProtocol)
                Array.Resize<byte>(ref array, index + 2 + 1);
              else
                Array.Resize<byte>(ref array, index * 2 + 4 + 1);
              this._lstDataPackets.Add(array);
              return;
            }
            if (this.Use8BitProtocol)
            {
              array[3 + index] = (byte) num2;
            }
            else
            {
              array[5 + index * 2] = (byte) ((num2 & 240) >> 4);
              array[6 + index * 2] = (byte) (num2 & 15);
            }
          }
          this._lstDataPackets.Add(array);
        }
      }
    }
  }
}
