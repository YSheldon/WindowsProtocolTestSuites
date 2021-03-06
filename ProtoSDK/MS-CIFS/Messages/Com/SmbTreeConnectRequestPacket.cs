// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Microsoft.Protocols.TestTools.StackSdk;
using Microsoft.Protocols.TestTools.StackSdk.Messages;

namespace Microsoft.Protocols.TestTools.StackSdk.FileAccessService.Cifs
{
    /// <summary>
    ///  Packets for SmbTreeConnect Request
    /// </summary>
    public class SmbTreeConnectRequestPacket : SmbSingleRequestPacket
    {
        #region Fields

        private SMB_COM_TREE_CONNECT_Request_SMB_Parameters smbParameters;
        private SMB_COM_TREE_CONNECT_Request_SMB_Data smbData;

        #endregion


        #region Properties

        /// <summary>
        /// get or set the Smb_Parameters:SMB_COM_TREE_CONNECT_Request_SMB_Parameters
        /// </summary>
        public SMB_COM_TREE_CONNECT_Request_SMB_Parameters SmbParameters
        {
            get
            {
                return this.smbParameters;
            }
            set
            {
                this.smbParameters = value;
            }
        }


        /// <summary>
        /// get or set the Smb_Data:SMB_COM_TREE_CONNECT_Request_SMB_Data
        /// </summary>
        public SMB_COM_TREE_CONNECT_Request_SMB_Data SmbData
        {
            get
            {
                return this.smbData;
            }
            set
            {
                this.smbData = value;
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SmbTreeConnectRequestPacket()
            : base()
        {
            this.InitDefaultValue();
        }


        /// <summary>
        /// Constructor: Create a request directly from a buffer.
        /// </summary>
        public SmbTreeConnectRequestPacket(byte[] data)
            : base(data)
        {
        }


        /// <summary>
        /// Deep copy constructor.
        /// </summary>
        public SmbTreeConnectRequestPacket(SmbTreeConnectRequestPacket packet)
            : base(packet)
        {
            this.InitDefaultValue();

            this.smbParameters.WordCount = packet.SmbParameters.WordCount;

            this.smbData.ByteCount = packet.SmbData.ByteCount;
            this.smbData.BufferFormat1 = packet.SmbData.BufferFormat1;
            if (packet.smbData.Path != null)
            {
                this.smbData.Path = new byte[packet.smbData.Path.Length];
                Array.Copy(packet.smbData.Path, this.smbData.Path, packet.smbData.Path.Length);
            }
            else
            {
                this.smbData.Path = new byte[0];
            }

            this.smbData.BufferFormat2 = packet.SmbData.BufferFormat2;
            if (packet.smbData.Password != null)
            {
                this.smbData.Password = new byte[packet.smbData.Password.Length];
                Array.Copy(packet.smbData.Password, this.smbData.Password, packet.smbData.Password.Length);
            }
            else
            {
                this.smbData.Password = new byte[0];
            }

            this.smbData.BufferFormat3 = packet.SmbData.BufferFormat3;
            if (packet.smbData.Service != null)
            {
                this.smbData.Service = new byte[packet.smbData.Service.Length];
                Array.Copy(packet.smbData.Service, this.smbData.Service, packet.smbData.Service.Length);
            }
            else
            {
                this.smbData.Service = new byte[0];
            }
        }

        #endregion


        #region override methods

        /// <summary>
        /// to create an instance of the StackPacket class that is identical to the current StackPacket. 
        /// </summary>
        /// <returns>a new Packet cloned from this.</returns>
        public override StackPacket Clone()
        {
            return new SmbTreeConnectRequestPacket(this);
        }


        /// <summary>
        /// Encode the struct of SMB_COM_TREE_CONNECT_Request_SMB_Parameters into the struct of SmbParameters
        /// </summary>
        protected override void EncodeParameters()
        {
            this.smbParametersBlock = TypeMarshal.ToStruct<SmbParameters>(
                CifsMessageUtils.ToBytes<SMB_COM_TREE_CONNECT_Request_SMB_Parameters>(this.smbParameters));
        }


        /// <summary>
        /// Encode the struct of SMB_COM_TREE_CONNECT_Request_SMB_Data into the struct of SmbData
        /// </summary>
        protected override void EncodeData()
        {
            this.smbDataBlock.ByteCount = this.SmbData.ByteCount;
            this.smbDataBlock.Bytes = new byte[this.smbDataBlock.ByteCount];
            using (MemoryStream memoryStream = new MemoryStream(this.smbDataBlock.Bytes))
            {
                using (Channel channel = new Channel(null, memoryStream))
                {

                    channel.BeginWriteGroup();
                    channel.Write<byte>(this.SmbData.BufferFormat1);
                    if (this.SmbData.Path != null)
                    {
                        channel.WriteBytes(this.SmbData.Path);
                    }
                    channel.Write<byte>(this.SmbData.BufferFormat2);
                    if (this.SmbData.Password != null)
                    {
                        channel.WriteBytes(this.SmbData.Password);
                    }
                    channel.Write<byte>(this.SmbData.BufferFormat3);
                    if (this.SmbData.Service != null)
                    {
                        channel.WriteBytes(this.SmbData.Service);
                    }
                    channel.EndWriteGroup();
                }
            }
        }


        /// <summary>
        /// to decode the smb parameters: from the general SmbParameters to the concrete Smb Parameters.
        /// </summary>
        protected override void DecodeParameters()
        {
            this.smbParameters = TypeMarshal.ToStruct<SMB_COM_TREE_CONNECT_Request_SMB_Parameters>(
                TypeMarshal.ToBytes(this.smbParametersBlock));
        }


        /// <summary>
        /// to decode the smb data: from the general SmbDada to the concrete Smb Data.
        /// </summary>
        protected override void DecodeData()
        {
            using (MemoryStream memoryStream = new MemoryStream(CifsMessageUtils.ToBytes<SmbData>(this.smbDataBlock)))
            {
                using (Channel channel = new Channel(null, memoryStream))
                {
                    this.smbData.ByteCount = channel.Read<ushort>();
                    this.smbData.BufferFormat1 = channel.Read<byte>();
                    this.smbData.Path = CifsMessageUtils.ReadNullTerminatedString(channel, false);
                    this.smbData.BufferFormat2 = channel.Read<byte>();
                    this.smbData.Password = CifsMessageUtils.ReadNullTerminatedString(channel, false);
                    this.smbData.BufferFormat3 = channel.Read<byte>();

                    // Service:
                    int serviceLen = this.smbDataBlock.Bytes.Length - this.smbData.Path.Length - this.smbData.Password.Length
                        - 3; // 3 is the bytes count of BufferFormat1, BufferFormat2 and BufferFormat3.
                    this.smbData.Service = channel.ReadBytes(serviceLen);
                }
            }

        }

        #endregion


        #region initialize fields with default value

        /// <summary>
        /// init packet, set default field data
        /// </summary>
        private void InitDefaultValue()
        {
            this.smbData.Password = new byte[0];
            this.smbData.Path = new byte[0];
            this.smbData.Service = new byte[0];
        }

        #endregion
    }
}
