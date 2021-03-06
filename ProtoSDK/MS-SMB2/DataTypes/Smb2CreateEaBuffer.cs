﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Microsoft.Protocols.TestTools.StackSdk.FileAccessService.Smb2
{
    public class Smb2CreateEaBuffer : Smb2CreateContextRequest
    {
        public List<FsccFileFullEaInformation> FileFullEaInformations;
    }
}
