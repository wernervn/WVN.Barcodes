using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using WVN.Barcodes.Common;
using WVN.Barcodes.Exceptions;

namespace WVN.Barcodes.Pdf417
{
    public sealed class Pdf417 : IBarcode
    {
        #region Properties

        private readonly CodeWord StartPattern = new StartWord();
        private readonly CodeWord EndPattern = new EndWord(); // only codeword with 18 modules
        private readonly List<EncodingTable> EncodingTables = new List<EncodingTable>();
        private readonly List<TextEncodingTable> TextMatrices = new List<TextEncodingTable>();
        private readonly List<List<int>> ErrorCodeFactors = new List<List<int>>();

        private int ModuleWidth { get; set; }

        #endregion

        #region ctor

        public Pdf417()
        {
            InitialiseTextMatrices();
            InitialiseEncodingTables();
            InitialiseErrorCodeFactors();
            ModuleWidth = 1;
        }

        public void Dispose()
        {
            EncodingTables[0].Clear();
            EncodingTables[1].Clear();
            EncodingTables[2].Clear();

            EncodingTables[0] = null;
            EncodingTables[1] = null;
            EncodingTables[2] = null;

            TextMatrices[0].Clear();
            TextMatrices[1].Clear();
            TextMatrices[2].Clear();
            TextMatrices[3].Clear();

            TextMatrices[0] = null;
            TextMatrices[1] = null;
            TextMatrices[2] = null;
            TextMatrices[3] = null;
        }

        #endregion

        #region Private Methods

        #region Initialisation

        #region EncodingTable Initialisation

        // Initialise exhaustive lists of encoding tables
        private void InitialiseEncodingTables()
        {
            EncodingTables.Insert(0, GetEncodingTable1());
            EncodingTables.Insert(1, GetEncodingTable2());
            EncodingTables.Insert(2, GetEncodingTable3());
        }

        private EncodingTable GetEncodingTable1()
        {
            #region Patterns

            var patterns = new List<int>
            {
                0x6ae0, 0x7578, 0x7abe, 0x6a70, 0x753c, 0x7a9f, 0x5460, 0x6a38, 0x5430, 0x2820, 0x5418, 0x2810, 0x56e0, 0x6b78,
                0x75be, 0x5670, 0x6b3c, 0x759f, 0x2c60, 0x5638, 0x2c30, 0x2ee0, 0x5778, 0x6bbe, 0x2e70, 0x573c, 0x6b9f, 0x2e38,
                0x571e, 0x2f78, 0x57be, 0x2f3c, 0x579f, 0x2fbe, 0x7afd, 0x6970, 0x74bc, 0x7a5f, 0x5260, 0x6938, 0x749e, 0x5230,
                0x691c, 0x2420, 0x5218, 0x690e, 0x2410, 0x520c, 0x2408, 0x5370, 0x69bc, 0x74df, 0x2660, 0x5338, 0x699e, 0x2630,
                0x531c, 0x698f, 0x2618, 0x530e, 0x2770, 0x53bc, 0x69df, 0x2738, 0x539e, 0x271c, 0x538f, 0x27bc, 0x53df, 0x279e,
                0x278f, 0x5160, 0x68b8, 0x745e, 0x5130, 0x689c, 0x744f, 0x2220, 0x5118, 0x688e, 0x2210, 0x510c, 0x2208, 0x2204,
                0x2360, 0x51b8, 0x68de, 0x2330, 0x519c, 0x68cf, 0x2318, 0x518e, 0x230c, 0x2306, 0x23b8, 0x51de, 0x239c, 0x51cf,
                0x238e, 0x23de, 0x50b0, 0x685c, 0x742f, 0x2120, 0x5098, 0x684e, 0x2110, 0x508c, 0x6847, 0x2108, 0x5086, 0x2104,
                0x5083, 0x21b0, 0x50dc, 0x686f, 0x2198, 0x50ce, 0x218c, 0x50c7, 0x2186, 0x2183, 0x50ef, 0x21c7, 0x20a0, 0x5058,
                0x682e, 0x2090, 0x504c, 0x6827, 0x2088, 0x5046, 0x2084, 0x5043, 0x2082, 0x20d8, 0x20cc, 0x20c6, 0x2050, 0x6817,
                0x5026, 0x5023, 0x2041, 0x6570, 0x72bc, 0x795f, 0x4a60, 0x6538, 0x729e, 0x4a30, 0x651c, 0x728f, 0x1420, 0x4a18,
                0x1410, 0x4b70, 0x65bc, 0x72df, 0x1660, 0x4b38, 0x659e, 0x1630, 0x4b1c, 0x1618, 0x160c, 0x1770, 0x4bbc, 0x65df,
                0x1738, 0x4b9e, 0x171c, 0x170e, 0x17bc, 0x4bdf, 0x179e, 0x17df, 0x6d60, 0x76b8, 0x7b5e, 0x6d30, 0x769c, 0x7b4f,
                0x5a20, 0x6d18, 0x768e, 0x5a10, 0x6d0c, 0x7687, 0x5a08, 0x6d06, 0x4960, 0x64b8, 0x725e, 0x5b60, 0x4930, 0x649c,
                0x724f, 0x5b30, 0x6d9c, 0x76cf, 0x3620, 0x1210, 0x490c, 0x6487, 0x3610, 0x5b0c, 0x3608, 0x1360, 0x49b8, 0x64de,
                0x3760, 0x1330, 0x499c, 0x64cf, 0x3730, 0x5b9c, 0x6dcf, 0x3718, 0x130c, 0x370c, 0x13b8, 0x49de, 0x37b8, 0x139c,
                0x49cf, 0x379c, 0x5bcf, 0x378e, 0x13de, 0x37de, 0x13cf, 0x37cf, 0x6cb0, 0x765c, 0x7b2f, 0x5920, 0x6c98, 0x764e,
                0x5910, 0x6c8c, 0x7647, 0x5908, 0x6c86, 0x5904, 0x5902, 0x48b0, 0x645c, 0x722f, 0x59b0, 0x4898, 0x644e, 0x3320,
                0x1110, 0x6cce, 0x6447, 0x3310, 0x1108, 0x4886, 0x3308, 0x5986, 0x4883, 0x1102, 0x11b0, 0x48dc, 0x646f, 0x33b0,
                0x1198, 0x48ce, 0x3398, 0x59ce, 0x48c7, 0x338c, 0x1186, 0x1183, 0x11dc, 0x48ef, 0x33dc, 0x11ce, 0x33ce, 0x11c7,
                0x33c7, 0x33ef, 0x58a0, 0x6c58, 0x762e, 0x5890, 0x6c4c, 0x7627, 0x5888, 0x6c46, 0x5884, 0x6c43, 0x5882, 0x5881,
                0x10a0, 0x4858, 0x642e, 0x31a0, 0x1090, 0x484c, 0x6427, 0x3190, 0x58cc, 0x6c67, 0x3188, 0x1084, 0x4843, 0x3184,
                0x58c3, 0x3182, 0x10d8, 0x486e, 0x31d8, 0x10cc, 0x4867, 0x31cc, 0x58e7, 0x31c6, 0x10c3, 0x31c3, 0x31ee, 0x31e7,
                0x5850, 0x6c2c, 0x7617, 0x5848, 0x6c26, 0x5844, 0x6c23, 0x5842, 0x5841, 0x1050, 0x482c, 0x6417, 0x30d0, 0x1048,
                0x4826, 0x30c8, 0x5866, 0x4823, 0x30c4, 0x1042, 0x30c2, 0x1041, 0x106c, 0x30ec, 0x30e6, 0x30e3, 0x6c16, 0x6c13,
                0x5821, 0x4816, 0x1024, 0x3064, 0x3062, 0x3061, 0x4560, 0x62b8, 0x715e, 0x4530, 0x629c, 0xa20, 0x4518, 0x628e,
                0xa10, 0x450c, 0xa08, 0xa04, 0xb60, 0x45b8, 0x62de, 0xb30, 0x459c, 0x62cf, 0xb18, 0x458e, 0xb0c, 0xb06,
                0xbb8, 0x45de, 0xb9c, 0x45cf, 0xb8e, 0xbde, 0xbcf, 0x66b0, 0x735c, 0x79af, 0x4d20, 0x6698, 0x734e, 0x4d10,
                0x668c, 0x7347, 0x4d08, 0x6686, 0x4d04, 0x6683, 0x44b0, 0x625c, 0x712f, 0x4db0, 0x4498, 0x624e, 0x1b20, 0x910,
                0x66ce, 0x6247, 0x1b10, 0x4d8c, 0x4486, 0x1b08, 0x904, 0x1b04, 0x9b0, 0x44dc, 0x626f, 0x1bb0, 0x998, 0x66ef,
                0x1b98, 0x4dce, 0x44c7, 0x1b8c, 0x986, 0x1b86, 0x9dc, 0x44ef, 0x1bdc, 0x9ce, 0x1bce, 0x9c7, 0x9ef, 0x1bef,
                0x6ea0, 0x7758, 0x7bae, 0x6e90, 0x774c, 0x7ba7, 0x6e88, 0x7746, 0x6e84, 0x7743, 0x6e82, 0x4ca0, 0x6658, 0x732e,
                0x5da0, 0x4c90, 0x776e, 0x7327, 0x5d90, 0x6ecc, 0x7767, 0x5d88, 0x4c84, 0x6643, 0x5d84, 0x6ec3, 0x4c81, 0x8a0,
                0x4458, 0x622e, 0x19a0, 0x890, 0x444c, 0x6227, 0x3ba0, 0x1990, 0x4ccc, 0x6667, 0x3b90, 0x5dcc, 0x6ee7, 0x4443,
                0x3b88, 0x1984, 0x4cc3, 0x3b84, 0x881, 0x8d8, 0x446e, 0x19d8, 0x8cc, 0x4467, 0x3bd8, 0x19cc, 0x4ce7, 0x3bcc,
                0x5de7, 0x8c3, 0x19c3, 0x8ee, 0x19ee, 0x8e7, 0x3bee, 0x19e7, 0x6e50, 0x772c, 0x7b97, 0x6e48, 0x7726, 0x6e44,
                0x7723, 0x6e42, 0x6e41, 0x4c50, 0x662c, 0x7317, 0x5cd0, 0x4c48, 0x7737, 0x5cc8, 0x6e66, 0x6623, 0x5cc4, 0x4c42,
                0x5cc2, 0x4c41, 0x5cc1, 0x850, 0x442c, 0x6217, 0x18d0, 0x848, 0x4426, 0x39d0, 0x18c8, 0x4c66, 0x4423, 0x39c8,
                0x5ce6, 0x842, 0x39c4, 0x18c2, 0x841, 0x18c1, 0x86c, 0x4437, 0x18ec, 0x866, 0x39ec, 0x18e6, 0x863, 0x39e6,
                0x18e3, 0x877, 0x39f7, 0x6e28, 0x7716, 0x6e24, 0x7713, 0x6e22, 0x6e21, 0x4c28, 0x6616, 0x5c68, 0x4c24, 0x6613,
                0x5c64, 0x6e33, 0x5c62, 0x4c21, 0x5c61, 0x828, 0x4416, 0x1868, 0x824, 0x4413, 0x38e8, 0x1864, 0x4c33, 0x38e4,
                0x5c73, 0x821, 0x38e2, 0x1861, 0x38e1, 0x1876, 0x38f6, 0x38f3, 0x770b, 0x6e11, 0x660b, 0x4c12, 0x4c11, 0x814,
                0x1834, 0x3874, 0x811, 0x1831, 0x42b0, 0x520, 0x4298, 0x510, 0x428c, 0x6147, 0x508, 0x4286, 0x504, 0x4283,
                0x5b0, 0x42dc, 0x616f, 0x598, 0x42ce, 0x58c, 0x42c7, 0x586, 0x583, 0x5dc, 0x42ef, 0x5ce, 0x5c7, 0x5ef,
                0x46a0, 0x6358, 0x71ae, 0x4690, 0x634c, 0x4688, 0x6346, 0x4684, 0x6343, 0x4682, 0x4a0, 0x4258, 0x612e, 0xda0,
                0x490, 0x636e, 0x6127, 0xd90, 0x46cc, 0x6367, 0xd88, 0x484, 0x4243, 0xd84, 0x46c3, 0x481, 0x4d8, 0x426e,
                0xdd8, 0x4cc, 0x4267, 0xdcc, 0x46e7, 0xdc6, 0x4c3, 0x4ee, 0xdee, 0x4e7, 0xde7, 0x6750, 0x73ac, 0x79d7,
                0x6748, 0x73a6, 0x6744, 0x73a3, 0x6742, 0x6741, 0x4650, 0x632c, 0x4ed0, 0x4648, 0x6326, 0x4ec8, 0x6766, 0x6323,
                0x4ec4, 0x4642, 0x4ec2, 0x4641, 0x4ec1, 0x450, 0x422c, 0xcd0, 0x448, 0x6337, 0x1dd0, 0xcc8, 0x4666, 0x4223,
                0x1dc8, 0x4ee6, 0x442, 0x1dc4, 0xcc2, 0x441, 0xcc1, 0x46c, 0x4237, 0xcec, 0x466, 0x1dec, 0xce6, 0x463,
                0x1de6, 0xce3, 0x477, 0xcf7, 0x1df7, 0x77a8, 0x7bd6, 0x77a4, 0x7bd3, 0x77a2, 0x77a1, 0x6728, 0x7396, 0x6f68,
                0x77b6, 0x7393, 0x6f64, 0x77b3, 0x6f62, 0x6721, 0x6f61, 0x4628, 0x6316, 0x4e68, 0x4624, 0x6313, 0x5ee8, 0x4e64,
                0x6733, 0x5ee4, 0x6f73, 0x4621, 0x5ee2, 0x4e61, 0x5ee1, 0x428, 0x4216, 0xc68, 0x424, 0x4213, 0x1ce8, 0xc64,
                0x4633, 0x3de8, 0x1ce4, 0x4e73, 0x421, 0x3de4, 0x5ef3, 0xc61, 0x3de2, 0x436, 0xc76, 0x433, 0x1cf6, 0xc73,
                0x3df6, 0x1cf3, 0x3df3, 0x7794, 0x7bcb, 0x7792, 0x7791, 0x6714, 0x738b, 0x6f34, 0x779b, 0x6f32, 0x6711, 0x6f31,
                0x4614, 0x630b, 0x4e34, 0x4612, 0x5e74, 0x4e32, 0x4611, 0x5e72, 0x4e31, 0x5e71, 0x414, 0x420b, 0xc34, 0x461b,
                0x1c74, 0xc32, 0x411, 0x3cf4, 0x1c72, 0xc31, 0x3cf2, 0x1c71, 0x3cf1, 0xc3b, 0x3cfb, 0x7789, 0x6f1a, 0x6f19,
                0x4e1a, 0x5e3a, 0x5e39, 0xc1a, 0x1c3a, 0x3c7a, 0x3c79, 0x2a0, 0x290, 0x414c, 0x288, 0x284, 0x282, 0x2d8,
                0x2cc, 0x2c6, 0x2c3, 0x2ee, 0x2e7, 0x4350, 0x4348, 0x61a6, 0x4344, 0x61a3, 0x4342, 0x4341, 0x250, 0x412c,
                0x6d0, 0x436c, 0x4126, 0x6c8, 0x4366, 0x6c4, 0x4363, 0x6c2, 0x241, 0x6c1, 0x26c, 0x4137, 0x6ec, 0x4377,
                0x6e6, 0x263, 0x6e3, 0x277, 0x6f7, 0x63a8, 0x63a4, 0x63a2, 0x63a1, 0x4328, 0x4768, 0x63b6, 0x6193, 0x4764,
                0x63b3, 0x4762, 0x4321, 0x4761, 0x228, 0x668, 0x224, 0x4113, 0xee8, 0x664, 0x222, 0xee4, 0x662, 0x221,
                0xee2, 0x661, 0x236, 0x676, 0x233, 0xef6, 0x673, 0xef3, 0x73d4, 0x73d2, 0x73d1, 0x6394, 0x67b4, 0x73db,
                0x67b2, 0x6391, 0x67b1, 0x4314, 0x618b, 0x4734, 0x639b, 0x4f74, 0x4732, 0x4311, 0x4f72, 0x4731, 0x4f71, 0x214,
                0x410b, 0x634, 0x431b, 0xe74, 0x632, 0x211, 0x1ef4, 0xe72, 0x631, 0x1ef2, 0xe71, 0x21b, 0x63b, 0xe7b,
                0x1efb, 0x7bea, 0x7be9, 0x73ca, 0x77da, 0x73c9, 0x77d9, 0x638a, 0x679a, 0x6389, 0x6fba, 0x6799, 0x6fb9, 0x430a,
                0x471a, 0x4309, 0x4f3a, 0x4719, 0x5f7a
            };

            #endregion
            var et = new EncodingTable();
            et.AddRange(patterns.Select(p => new CodeWord(p)));
            return et;
        }
        private EncodingTable GetEncodingTable2()
        {
            #region Patterns

            var patterns = new List<int>
            {
                0x7ab0, 0x7d5c, 0x7520, 0x7a98, 0x7d4e, 0x7510, 0x7a8c, 0x7d47, 0x7508, 0x7a86, 0x7504, 0x7a83, 0x7502, 0x75b0,
                0x7adc, 0x7d6f, 0x6b20, 0x7598, 0x7ace, 0x6b10, 0x758c, 0x7ac7, 0x6b08, 0x7586, 0x6b04, 0x7583, 0x6b02, 0x6bb0,
                0x75dc, 0x7aef, 0x5720, 0x6b98, 0x75ce, 0x5710, 0x6b8c, 0x75c7, 0x5708, 0x6b86, 0x5704, 0x6b83, 0x5702, 0x57b0,
                0x6bdc, 0x75ef, 0x2f20, 0x5798, 0x6bce, 0x2f10, 0x578c, 0x6bc7, 0x2f08, 0x5786, 0x2f04, 0x5783, 0x2fb0, 0x57dc,
                0x6bef, 0x2f98, 0x57ce, 0x2f8c, 0x57c7, 0x2f86, 0x2fdc, 0x57ef, 0x2fce, 0x2fc7, 0x74a0, 0x7a58, 0x7d2e, 0x7490,
                0x7a4c, 0x7d27, 0x7488, 0x7a46, 0x7484, 0x7a43, 0x7482, 0x7481, 0x69a0, 0x74d8, 0x7a6e, 0x6990, 0x74cc, 0x7a67,
                0x6988, 0x74c6, 0x6984, 0x74c3, 0x6982, 0x6981, 0x53a0, 0x69d8, 0x74ee, 0x5390, 0x69cc, 0x74e7, 0x5388, 0x69c6,
                0x5384, 0x69c3, 0x5382, 0x5381, 0x27a0, 0x53d8, 0x69ee, 0x2790, 0x53cc, 0x69e7, 0x2788, 0x53c6, 0x2784, 0x53c3,
                0x2782, 0x27d8, 0x53ee, 0x27cc, 0x53e7, 0x27c6, 0x27c3, 0x27ee, 0x27e7, 0x7450, 0x7a2c, 0x7d17, 0x7448, 0x7a26,
                0x7444, 0x7a23, 0x7442, 0x7441, 0x68d0, 0x746c, 0x7a37, 0x68c8, 0x7466, 0x68c4, 0x7463, 0x68c2, 0x68c1, 0x51d0,
                0x68ec, 0x7477, 0x51c8, 0x68e6, 0x51c4, 0x68e3, 0x51c2, 0x51c1, 0x23d0, 0x51ec, 0x68f7, 0x23c8, 0x51e6, 0x23c4,
                0x51e3, 0x23c2, 0x23c1, 0x23ec, 0x51f7, 0x23e6, 0x23e3, 0x23f7, 0x7428, 0x7a16, 0x7424, 0x7a13, 0x7422, 0x7421,
                0x6868, 0x7436, 0x6864, 0x7433, 0x6862, 0x6861, 0x50e8, 0x6876, 0x50e4, 0x6873, 0x50e2, 0x50e1, 0x21e8, 0x50f6,
                0x21e4, 0x50f3, 0x21e2, 0x21e1, 0x21f6, 0x21f3, 0x7414, 0x7a0b, 0x7412, 0x7411, 0x6834, 0x741b, 0x6832, 0x6831,
                0x5074, 0x683b, 0x5072, 0x5071, 0x20f4, 0x507b, 0x20f2, 0x20f1, 0x740a, 0x7409, 0x681a, 0x6819, 0x503a, 0x5039,
                0x72a0, 0x7958, 0x7cae, 0x7290, 0x794c, 0x7ca7, 0x7288, 0x7946, 0x7284, 0x7943, 0x7282, 0x7281, 0x65a0, 0x72d8,
                0x796e, 0x6590, 0x72cc, 0x7967, 0x6588, 0x72c6, 0x6584, 0x72c3, 0x6582, 0x6581, 0x4ba0, 0x65d8, 0x72ee, 0x4b90,
                0x65cc, 0x72e7, 0x4b88, 0x65c6, 0x4b84, 0x65c3, 0x4b82, 0x4b81, 0x17a0, 0x4bd8, 0x65ee, 0x1790, 0x4bcc, 0x65e7,
                0x1788, 0x4bc6, 0x1784, 0x4bc3, 0x1782, 0x17d8, 0x4bee, 0x17cc, 0x4be7, 0x17c6, 0x17c3, 0x17ee, 0x17e7, 0x7b50,
                0x7dac, 0x35f8, 0x7b48, 0x7da6, 0x34fc, 0x7b44, 0x7da3, 0x347e, 0x7b42, 0x7b41, 0x7250, 0x792c, 0x7c97, 0x76d0,
                0x7248, 0x7db7, 0x76c8, 0x7b66, 0x7923, 0x76c4, 0x7242, 0x76c2, 0x7241, 0x76c1, 0x64d0, 0x726c, 0x7937, 0x6dd0,
                0x64c8, 0x7266, 0x6dc8, 0x76e6, 0x7263, 0x6dc4, 0x64c2, 0x6dc2, 0x64c1, 0x6dc1, 0x49d0, 0x64ec, 0x7277, 0x5bd0,
                0x49c8, 0x64e6, 0x5bc8, 0x6de6, 0x64e3, 0x5bc4, 0x49c2, 0x5bc2, 0x49c1, 0x5bc1, 0x13d0, 0x49ec, 0x64f7, 0x37d0,
                0x13c8, 0x49e6, 0x37c8, 0x5be6, 0x49e3, 0x37c4, 0x13c2, 0x37c2, 0x13c1, 0x13ec, 0x49f7, 0x37ec, 0x13e6, 0x37e6,
                0x13e3, 0x37e3, 0x13f7, 0x7b28, 0x7d96, 0x32fc, 0x7b24, 0x7d93, 0x327e, 0x7b22, 0x323f, 0x7b21, 0x7228, 0x7916,
                0x7668, 0x7224, 0x7913, 0x7664, 0x7b33, 0x7662, 0x7221, 0x7661, 0x6468, 0x7236, 0x6ce8, 0x6464, 0x7233, 0x6ce4,
                0x7673, 0x6ce2, 0x6461, 0x6ce1, 0x48e8, 0x6476, 0x59e8, 0x48e4, 0x6473, 0x59e4, 0x6cf3, 0x59e2, 0x48e1, 0x59e1,
                0x11e8, 0x48f6, 0x33e8, 0x11e4, 0x48f3, 0x33e4, 0x59f3, 0x33e2, 0x11e1, 0x33e1, 0x11f6, 0x33f6, 0x11f3, 0x33f3,
                0x7b14, 0x7d8b, 0x317e, 0x7b12, 0x313f, 0x7b11, 0x7214, 0x790b, 0x7634, 0x7b1b, 0x7632, 0x7211, 0x7631, 0x6434,
                0x721b, 0x6c74, 0x6432, 0x6c72, 0x6431, 0x6c71, 0x4874, 0x643b, 0x58f4, 0x6c7b, 0x58f2, 0x4871, 0x58f1, 0x10f4,
                0x487b, 0x31f4, 0x10f2, 0x31f2, 0x10f1, 0x31f1, 0x10fb, 0x31fb, 0x7b0a, 0x30bf, 0x7b09, 0x720a, 0x761a, 0x7209,
                0x7619, 0x641a, 0x6c3a, 0x6419, 0x6c39, 0x483a, 0x587a, 0x4839, 0x5879, 0x107a, 0x30fa, 0x1079, 0x30f9, 0x7b05,
                0x7205, 0x760d, 0x640d, 0x6c1d, 0x481d, 0x583d, 0x7150, 0x78ac, 0x7c57, 0x7148, 0x78a6, 0x7144, 0x78a3, 0x7142,
                0x7141, 0x62d0, 0x716c, 0x78b7, 0x62c8, 0x7166, 0x62c4, 0x7163, 0x62c2, 0x62c1, 0x45d0, 0x62ec, 0x7177, 0x45c8,
                0x62e6, 0x45c4, 0x62e3, 0x45c2, 0x45c1, 0xbd0, 0x45ec, 0x62f7, 0xbc8, 0x45e6, 0xbc4, 0x45e3, 0xbc2, 0xbc1,
                0xbec, 0x45f7, 0xbe6, 0xbe3, 0xbf7, 0x79a8, 0x7cd6, 0x1afc, 0x79a4, 0x7cd3, 0x1a7e, 0x79a2, 0x1a3f, 0x79a1,
                0x7128, 0x7896, 0x7368, 0x7124, 0x7893, 0x7364, 0x79b3, 0x7362, 0x7121, 0x7361, 0x6268, 0x7136, 0x66e8, 0x6264,
                0x7133, 0x66e4, 0x7373, 0x66e2, 0x6261, 0x66e1, 0x44e8, 0x6276, 0x4de8, 0x44e4, 0x6273, 0x4de4, 0x66f3, 0x4de2,
                0x44e1, 0x4de1, 0x9e8, 0x44f6, 0x1be8, 0x9e4, 0x44f3, 0x1be4, 0x4df3, 0x1be2, 0x9e1, 0x1be1, 0x9f6, 0x1bf6,
                0x9f3, 0x1bf3, 0x7dd4, 0x3af8, 0x5d7e, 0x7dd2, 0x3a7c, 0x5d3f, 0x7dd1, 0x3a3e, 0x3a1f, 0x7994, 0x7ccb, 0x197e,
                0x7bb4, 0x7ddb, 0x3b7e, 0x193f, 0x7bb2, 0x7991, 0x3b3f, 0x7bb1, 0x7114, 0x788b, 0x7334, 0x7112, 0x7774, 0x7bbb,
                0x7111, 0x7772, 0x7331, 0x7771, 0x6234, 0x711b, 0x6674, 0x6232, 0x6ef4, 0x6672, 0x6231, 0x6ef2, 0x6671, 0x6ef1,
                0x4474, 0x623b, 0x4cf4, 0x4472, 0x5df4, 0x4cf2, 0x4471, 0x5df2, 0x4cf1, 0x5df1, 0x8f4, 0x447b, 0x19f4, 0x8f2,
                0x3bf4, 0x19f2, 0x8f1, 0x3bf2, 0x19f1, 0x3bf1, 0x8fb, 0x19fb, 0x7dca, 0x397c, 0x5cbf, 0x7dc9, 0x393e, 0x391f,
                0x798a, 0x18bf, 0x7b9a, 0x7989, 0x39bf, 0x7b99, 0x710a, 0x731a, 0x7109, 0x773a, 0x7319, 0x7739, 0x621a, 0x663a,
                0x6219, 0x6e7a, 0x6639, 0x6e79, 0x443a, 0x4c7a, 0x4439, 0x5cfa, 0x4c79, 0x5cf9, 0x87a, 0x18fa, 0x879, 0x39fa,
                0x18f9, 0x39f9, 0x7dc5, 0x38be, 0x389f, 0x7985, 0x7b8d, 0x7105, 0x730d, 0x771d, 0x620d, 0x661d, 0x6e3d, 0x441d,
                0x4c3d, 0x5c7d, 0x83d, 0x187d, 0x38fd, 0x385f, 0x70a8, 0x7856, 0x70a4, 0x7853, 0x70a2, 0x70a1, 0x6168, 0x70b6,
                0x6164, 0x70b3, 0x6162, 0x6161, 0x42e8, 0x6176, 0x42e4, 0x6173, 0x42e2, 0x42e1, 0x5e8, 0x42f6, 0x5e4, 0x42f3,
                0x5e2, 0x5e1, 0x5f6, 0x5f3, 0x78d4, 0x7c6b, 0xd7e, 0x78d2, 0xd3f, 0x78d1, 0x7094, 0x784b, 0x71b4, 0x7092,
                0x71b2, 0x7091, 0x71b1, 0x6134, 0x709b, 0x6374, 0x6132, 0x6372, 0x6131, 0x6371, 0x4274, 0x613b, 0x46f4, 0x4272,
                0x46f2, 0x4271, 0x46f1, 0x4f4, 0x427b, 0xdf4, 0x4f2, 0xdf2, 0x4f1, 0xdf1, 0x4fb, 0xdfb, 0x7cea, 0x1d7c,
                0x4ebf, 0x7ce9, 0x1d3e, 0x1d1f, 0x78ca, 0xcbf, 0x79da, 0x78c9, 0x1dbf, 0x79d9, 0x708a, 0x719a, 0x7089, 0x73ba,
                0x7199, 0x73b9, 0x611a, 0x633a, 0x6119, 0x677a, 0x6339, 0x6779, 0x423a, 0x467a, 0x4239, 0x4efa, 0x4679, 0x4ef9,
                0x47a, 0xcfa, 0x479, 0x1dfa, 0xcf9, 0x1df9, 0x3d78, 0x5ebe, 0x3d3c, 0x5e9f, 0x3d1e, 0x3d0f, 0x7ce5, 0x1cbe,
                0x7ded, 0x3dbe, 0x1c9f, 0x3d9f, 0x78c5, 0x79cd, 0x7bdd, 0x7085, 0x718d, 0x739d, 0x77bd, 0x610d, 0x631d, 0x673d,
                0x6f7d, 0x421d, 0x463d, 0x4e7d, 0x5efd, 0x43d, 0xc7d, 0x1cfd, 0x3cbc, 0x5e5f, 0x3c9e, 0x3c8f, 0x1c5f, 0x3cdf,
                0x3c5e, 0x3c4f, 0x3c2f, 0x7054, 0x7052, 0x7051, 0x60b4, 0x705b, 0x60b2, 0x60b1, 0x4174, 0x60bb, 0x4172, 0x4171,
                0x2f4, 0x417b, 0x2f2, 0x2f1, 0x2fb, 0x786a, 0x6bf, 0x7869, 0x704a, 0x70da, 0x7049, 0x70d9, 0x609a, 0x61ba,
                0x6099, 0x61b9, 0x413a, 0x437a, 0x4139, 0x4379, 0x27a, 0x6fa, 0x279, 0x6f9, 0x7c75, 0xebe, 0xe9f, 0x7865,
                0x78ed, 0x7045, 0x70cd, 0x71dd, 0x608d, 0x619d, 0x63bd, 0x411d, 0x433d, 0x477d, 0x23d, 0x67d, 0xefd, 0x1ebc,
                0x4f5f, 0x1e9e, 0x1e8f, 0xe5f, 0x1edf, 0x3eb8, 0x5f5e, 0x3e9c, 0x5f4f, 0x3e8e, 0x3e87, 0x1e5e, 0x3ede, 0x1e4f,
                0x3ecf, 0x3e5c, 0x5f2f, 0x3e4e, 0x3e47, 0x1e2f, 0x3e6f, 0x3e2e, 0x3e27, 0x3e17, 0x605a, 0x6059, 0x40ba, 0x40b9,
                0x17a, 0x179, 0x706d, 0x604d, 0x60dd, 0x409d, 0x41bd, 0x13d, 0x37d, 0x75f, 0xf5e, 0xf4f, 0x1f5c, 0x4faf,
                0x1f4e, 0x1f47, 0xf2f, 0x1f6f, 0x3f58, 0x5fae, 0x3f4c, 0x5fa7, 0x3f46, 0x3f43, 0x1f2e, 0x3f6e, 0x1f27, 0x3f67,
                0x3f2c, 0x5f97, 0x3f26, 0x3f23, 0x1f17, 0x3f37, 0x3f16, 0x3f13, 0x7af, 0xfae, 0xfa7, 0x1fac, 0x4fd7, 0x1fa6,
                0x1fa3, 0xf97, 0x1fb7, 0x1f96, 0x1f93
            };

            #endregion
            var et = new EncodingTable();
            et.AddRange(patterns.Select(p => new CodeWord(p)));
            return et;
        }
        private EncodingTable GetEncodingTable3()
        {
            #region Patterns

            var patterns = new List<int>
            {
                0x55f0, 0x6afc, 0x29e0, 0x54f8, 0x6a7e, 0x28f0, 0x547c, 0x6a3f, 0x2878, 0x543e, 0x283c, 0x7d68, 0x2df0, 0x56fc,
                0x7d64, 0x2cf8, 0x567e, 0x7d62, 0x2c7c, 0x563f, 0x7d61, 0x2c3e, 0x7ae8, 0x7d76, 0x2efc, 0x7ae4, 0x7d73, 0x2e7e,
                0x7ae2, 0x2e3f, 0x7ae1, 0x75e8, 0x7af6, 0x75e4, 0x7af3, 0x75e2, 0x75e1, 0x6be8, 0x75f6, 0x6be4, 0x75f3, 0x6be2,
                0x6be1, 0x57e8, 0x6bf6, 0x57e4, 0x6bf3, 0x57e2, 0x25e0, 0x52f8, 0x697e, 0x24f0, 0x527c, 0x693f, 0x2478, 0x523e,
                0x243c, 0x521f, 0x241e, 0x7d34, 0x26f8, 0x537e, 0x7d32, 0x267c, 0x533f, 0x7d31, 0x263e, 0x261f, 0x7a74, 0x7d3b,
                0x277e, 0x7a72, 0x273f, 0x7a71, 0x74f4, 0x7a7b, 0x74f2, 0x74f1, 0x69f4, 0x74fb, 0x69f2, 0x69f1, 0x53f4, 0x69fb,
                0x53f2, 0x53f1, 0x22f0, 0x517c, 0x68bf, 0x2278, 0x513e, 0x223c, 0x511f, 0x221e, 0x220f, 0x7d1a, 0x237c, 0x51bf,
                0x7d19, 0x233e, 0x231f, 0x7a3a, 0x23bf, 0x7a39, 0x747a, 0x7479, 0x68fa, 0x68f9, 0x51fa, 0x51f9, 0x2178, 0x50be,
                0x213c, 0x509f, 0x211e, 0x210f, 0x7d0d, 0x21be, 0x219f, 0x7a1d, 0x743d, 0x687d, 0x20bc, 0x505f, 0x209e, 0x208f,
                0x20df, 0x205e, 0x204f, 0x15e0, 0x4af8, 0x657e, 0x14f0, 0x4a7c, 0x653f, 0x1478, 0x4a3e, 0x143c, 0x4a1f, 0x141e,
                0x7cb4, 0x16f8, 0x4b7e, 0x7cb2, 0x167c, 0x4b3f, 0x7cb1, 0x163e, 0x161f, 0x7974, 0x7cbb, 0x177e, 0x7972, 0x173f,
                0x7971, 0x72f4, 0x797b, 0x72f2, 0x72f1, 0x65f4, 0x72fb, 0x65f2, 0x65f1, 0x4bf4, 0x65fb, 0x4bf2, 0x4bf1, 0x5af0,
                0x6d7c, 0x76bf, 0x34e0, 0x5a78, 0x6d3e, 0x3470, 0x5a3c, 0x6d1f, 0x3438, 0x5a1e, 0x341c, 0x5a0f, 0x340e, 0x12f0,
                0x497c, 0x64bf, 0x36f0, 0x1278, 0x493e, 0x3678, 0x5b3e, 0x491f, 0x363c, 0x121e, 0x361e, 0x120f, 0x360f, 0x7c9a,
                0x137c, 0x49bf, 0x7dba, 0x7c99, 0x377c, 0x133e, 0x7db9, 0x373e, 0x131f, 0x371f, 0x793a, 0x13bf, 0x7b7a, 0x7939,
                0x37bf, 0x7b79, 0x727a, 0x76fa, 0x7279, 0x76f9, 0x64fa, 0x6dfa, 0x64f9, 0x6df9, 0x49fa, 0x49f9, 0x32e0, 0x5978,
                0x6cbe, 0x3270, 0x593c, 0x6c9f, 0x3238, 0x591e, 0x321c, 0x590f, 0x320e, 0x3207, 0x1178, 0x48be, 0x3378, 0x113c,
                0x489f, 0x333c, 0x599f, 0x331e, 0x110f, 0x330f, 0x7c8d, 0x11be, 0x7d9d, 0x33be, 0x119f, 0x339f, 0x791d, 0x7b3d,
                0x723d, 0x767d, 0x647d, 0x6cfd, 0x48fd, 0x3170, 0x58bc, 0x6c5f, 0x3138, 0x589e, 0x311c, 0x588f, 0x310e, 0x3107,
                0x10bc, 0x485f, 0x31bc, 0x109e, 0x319e, 0x108f, 0x318f, 0x10df, 0x31df, 0x30b8, 0x585e, 0x309c, 0x584f, 0x308e,
                0x3087, 0x105e, 0x30de, 0x104f, 0x30cf, 0x305c, 0x582f, 0x304e, 0x3047, 0x102f, 0x306f, 0x302e, 0x3027, 0xaf0,
                0x457c, 0x62bf, 0xa78, 0x453e, 0xa3c, 0x451f, 0xa1e, 0xa0f, 0x7c5a, 0xb7c, 0x45bf, 0x7c59, 0xb3e, 0xb1f,
                0x78ba, 0xbbf, 0x78b9, 0x717a, 0x7179, 0x62fa, 0x62f9, 0x45fa, 0x45f9, 0x1ae0, 0x4d78, 0x66be, 0x1a70, 0x4d3c,
                0x669f, 0x1a38, 0x4d1e, 0x1a1c, 0x4d0f, 0x1a0e, 0x1a07, 0x978, 0x44be, 0x1b78, 0x93c, 0x449f, 0x1b3c, 0x4d9f,
                0x1b1e, 0x90f, 0x1b0f, 0x7c4d, 0x9be, 0x7cdd, 0x1bbe, 0x99f, 0x1b9f, 0x789d, 0x79bd, 0x713d, 0x737d, 0x627d,
                0x66fd, 0x44fd, 0x5d70, 0x6ebc, 0x775f, 0x3a60, 0x5d38, 0x6e9e, 0x3a30, 0x5d1c, 0x6e8f, 0x3a18, 0x5d0e, 0x3a0c,
                0x5d07, 0x3a06, 0x1970, 0x4cbc, 0x665f, 0x3b70, 0x1938, 0x4c9e, 0x3b38, 0x5d9e, 0x4c8f, 0x3b1c, 0x190e, 0x3b0e,
                0x1907, 0x3b07, 0x8bc, 0x445f, 0x19bc, 0x89e, 0x3bbc, 0x199e, 0x88f, 0x3b9e, 0x198f, 0x3b8f, 0x8df, 0x19df,
                0x3bdf, 0x3960, 0x5cb8, 0x6e5e, 0x3930, 0x5c9c, 0x6e4f, 0x3918, 0x5c8e, 0x390c, 0x5c87, 0x3906, 0x3903, 0x18b8,
                0x4c5e, 0x39b8, 0x189c, 0x4c4f, 0x399c, 0x5ccf, 0x398e, 0x1887, 0x3987, 0x85e, 0x18de, 0x84f, 0x39de, 0x18cf,
                0x39cf, 0x38b0, 0x5c5c, 0x6e2f, 0x3898, 0x5c4e, 0x388c, 0x5c47, 0x3886, 0x3883, 0x185c, 0x4c2f, 0x38dc, 0x184e,
                0x38ce, 0x1847, 0x38c7, 0x82f, 0x186f, 0x38ef, 0x3858, 0x5c2e, 0x384c, 0x5c27, 0x3846, 0x3843, 0x182e, 0x386e,
                0x1827, 0x3867, 0x382c, 0x5c17, 0x3826, 0x3823, 0x1817, 0x3837, 0x3816, 0x3813, 0x578, 0x42be, 0x53c, 0x429f,
                0x51e, 0x50f, 0x5be, 0x59f, 0x785d, 0x70bd, 0x617d, 0x42fd, 0xd70, 0x46bc, 0x635f, 0xd38, 0x469e, 0xd1c,
                0x468f, 0xd0e, 0xd07, 0x4bc, 0x425f, 0xdbc, 0x49e, 0xd9e, 0x48f, 0xd8f, 0x4df, 0xddf, 0x1d60, 0x4eb8,
                0x675e, 0x1d30, 0x4e9c, 0x674f, 0x1d18, 0x4e8e, 0x1d0c, 0x4e87, 0x1d06, 0x1d03, 0xcb8, 0x465e, 0x1db8, 0xc9c,
                0x464f, 0x1d9c, 0xc8e, 0x1d8e, 0xc87, 0x1d87, 0x45e, 0xcde, 0x44f, 0x1dde, 0xccf, 0x1dcf, 0x5eb0, 0x6f5c,
                0x77af, 0x3d20, 0x5e98, 0x6f4e, 0x3d10, 0x5e8c, 0x6f47, 0x3d08, 0x5e86, 0x3d04, 0x5e83, 0x3d02, 0x1cb0, 0x4e5c,
                0x672f, 0x3db0, 0x1c98, 0x4e4e, 0x3d98, 0x5ece, 0x4e47, 0x3d8c, 0x1c86, 0x3d86, 0x1c83, 0x3d83, 0xc5c, 0x462f,
                0x1cdc, 0xc4e, 0x3ddc, 0x1cce, 0xc47, 0x3dce, 0x1cc7, 0x3dc7, 0x42f, 0xc6f, 0x1cef, 0x3def, 0x3ca0, 0x5e58,
                0x6f2e, 0x3c90, 0x5e4c, 0x6f27, 0x3c88, 0x5e46, 0x3c84, 0x5e43, 0x3c82, 0x3c81, 0x1c58, 0x4e2e, 0x3cd8, 0x1c4c,
                0x4e27, 0x3ccc, 0x5e67, 0x3cc6, 0x1c43, 0x3cc3, 0xc2e, 0x1c6e, 0xc27, 0x3cee, 0x1c67, 0x3ce7, 0x3c50, 0x5e2c,
                0x6f17, 0x3c48, 0x5e26, 0x3c44, 0x5e23, 0x3c42, 0x3c41, 0x1c2c, 0x4e17, 0x3c6c, 0x1c26, 0x3c66, 0x1c23, 0x3c63,
                0xc17, 0x1c37, 0x3c77, 0x3c28, 0x5e16, 0x3c24, 0x5e13, 0x3c22, 0x3c21, 0x1c16, 0x3c36, 0x1c13, 0x3c33, 0x3c14,
                0x5e0b, 0x3c12, 0x3c11, 0x1c0b, 0x3c1b, 0x2bc, 0x415f, 0x29e, 0x28f, 0x2df, 0x6b8, 0x435e, 0x69c, 0x434f,
                0x68e, 0x687, 0x25e, 0x6de, 0x24f, 0x6cf, 0xeb0, 0x475c, 0x63af, 0xe98, 0x474e, 0xe8c, 0x4747, 0xe86,
                0xe83, 0x65c, 0x432f, 0xedc, 0x64e, 0xece, 0x647, 0xec7, 0x22f, 0x66f, 0xeef, 0x1ea0, 0x4f58, 0x67ae,
                0x1e90, 0x4f4c, 0x67a7, 0x1e88, 0x4f46, 0x1e84, 0x4f43, 0x1e82, 0x1e81, 0xe58, 0x472e, 0x1ed8, 0xe4c, 0x4727,
                0x1ecc, 0x4f67, 0x1ec6, 0xe43, 0x1ec3, 0x62e, 0xe6e, 0x627, 0x1eee, 0xe67, 0x1ee7, 0x5f50, 0x6fac, 0x77d7,
                0x5f48, 0x6fa6, 0x5f44, 0x6fa3, 0x5f42, 0x5f41, 0x1e50, 0x4f2c, 0x6797, 0x3ed0, 0x1e48, 0x4f26, 0x3ec8, 0x5f66,
                0x4f23, 0x3ec4, 0x1e42, 0x3ec2, 0x1e41, 0x3ec1, 0xe2c, 0x4717, 0x1e6c, 0xe26, 0x3eec, 0x1e66, 0xe23, 0x3ee6,
                0x1e63, 0x3ee3, 0x617, 0xe37, 0x1e77, 0x3ef7, 0x5f28, 0x6f96, 0x5f24, 0x6f93, 0x5f22, 0x5f21, 0x1e28, 0x4f16,
                0x3e68, 0x1e24, 0x4f13, 0x3e64, 0x5f33, 0x3e62, 0x1e21, 0x3e61, 0xe16, 0x1e36, 0xe13, 0x3e76, 0x1e33, 0x3e73,
                0x5f14, 0x6f8b, 0x5f12, 0x5f11, 0x1e14, 0x4f0b, 0x3e34, 0x1e12, 0x3e32, 0x1e11, 0x3e31, 0xe0b, 0x1e1b, 0x3e3b,
                0x5f0a, 0x5f09, 0x1e0a, 0x3e1a, 0x1e09, 0x3e19, 0x15e, 0x14f, 0x35c, 0x41af, 0x34e, 0x347, 0x12f, 0x36f,
                0x758, 0x43ae, 0x74c, 0x43a7, 0x746, 0x743, 0x32e, 0x76e, 0x327, 0x767, 0xf50, 0x47ac, 0x63d7, 0xf48,
                0x47a6, 0xf44, 0x47a3, 0xf42, 0xf41, 0x72c, 0x4397, 0xf6c, 0x47b7, 0xf66, 0x723, 0xf63, 0x317, 0x737,
                0xf77, 0x4fa8, 0x67d6, 0x4fa4, 0x67d3, 0x4fa2, 0x4fa1, 0xf28, 0x4796, 0x1f68, 0x4fb6, 0x4793, 0x1f64, 0xf22,
                0x1f62, 0xf21, 0x1f61, 0x716, 0xf36, 0x713, 0x1f76, 0xf33, 0x1f73, 0x6fd4, 0x77eb, 0x6fd2, 0x6fd1, 0x4f94,
                0x67cb, 0x5fb4, 0x4f92, 0x5fb2, 0x4f91, 0x5fb1, 0xf14, 0x478b, 0x1f34, 0xf12, 0x3f74, 0x1f32, 0xf11, 0x3f72,
                0x1f31, 0x3f71, 0x70b, 0xf1b, 0x1f3b, 0x3f7b, 0x6fca, 0x6fc9, 0x4f8a, 0x5f9a, 0x4f89, 0x5f99, 0xf0a, 0x1f1a,
                0xf09, 0x3f3a, 0x1f19, 0x3f39, 0x6fc5, 0x4f85, 0x5f8d, 0xf05, 0x1f0d, 0x3f1d, 0x1ae, 0x1a7, 0x3ac, 0x41d7,
                0x3a6, 0x3a3, 0x197, 0x3b7, 0x7a8, 0x43d6, 0x7a4, 0x43d3, 0x7a2, 0x7a1, 0x396, 0x7b6, 0x393, 0x7b3,
                0x47d4, 0x63eb, 0x47d2, 0x47d1, 0x794, 0x43cb, 0xfb4, 0x47db, 0xfb2, 0x791, 0xfb1, 0x38b, 0x79b, 0xfbb,
                0x67ea, 0x67e9, 0x47ca, 0x4fda, 0x47c9, 0x4fd9, 0x78a, 0xf9a, 0x789, 0x1fba, 0xf99, 0x1fb9, 0x67e5, 0x47c5,
                0x4fcd, 0x785, 0xf8d, 0x1f9d, 0x1d6, 0x1d3, 0x3d4, 0x41eb, 0x3d2, 0x3d1, 0x1cb, 0x3db, 0x43ea, 0x43e9,
                0x3ca, 0x7da, 0x3c9, 0x7d9, 0x63f5
            };

            #endregion
            var et = new EncodingTable();
            et.AddRange(patterns.Select(p => new CodeWord(p)));
            return et;
        }

        #endregion

        private void InitialiseTextMatrices()
        {
            var uppercase = new TextEncodingTable();
            var lowercase = new TextEncodingTable();
            for (var i = 0; i < 26; i++)
            {
                uppercase.Add(new TextEncoding((char)(i + 65), i));
                lowercase.Add(new TextEncoding((char)(i + 97), i));
            }
            uppercase.Add(new TextEncoding(' ', 26));
            lowercase.Add(new TextEncoding(' ', 26));

            uppercase.Add(new TextEncoding(Switch.Lowercase, 27));
            lowercase.Add(new TextEncoding(Switch.UppercaseNextOnly, 27));

            uppercase.Add(new TextEncoding(Switch.Mixed, 28));
            lowercase.Add(new TextEncoding(Switch.Mixed, 28));

            uppercase.Add(new TextEncoding(Switch.PunctuationNextOnly, 29));
            lowercase.Add(new TextEncoding(Switch.PunctuationNextOnly, 29));

            TextMatrices.Insert(0, uppercase);
            TextMatrices.Insert(1, lowercase);

            var mixed = new TextEncodingTable();
            for (var i = 0; i < 10; i++)
            {
                mixed.Add(new TextEncoding((char)(i + 48), i));
            }
            mixed.Add(new TextEncoding('&', 10));
            mixed.Add(new TextEncoding('\r', 11));
            mixed.Add(new TextEncoding('\t', 12));
            mixed.Add(new TextEncoding(',', 13));
            mixed.Add(new TextEncoding(':', 14));
            mixed.Add(new TextEncoding('#', 15));
            mixed.Add(new TextEncoding('-', 16));
            mixed.Add(new TextEncoding('.', 17));
            mixed.Add(new TextEncoding('$', 18));
            mixed.Add(new TextEncoding('/', 19));
            mixed.Add(new TextEncoding('+', 20));
            mixed.Add(new TextEncoding('%', 21));
            mixed.Add(new TextEncoding('*', 22));
            mixed.Add(new TextEncoding('=', 23));
            mixed.Add(new TextEncoding('^', 24));
            mixed.Add(new TextEncoding(Switch.Punctuation, 25));
            mixed.Add(new TextEncoding(' ', 26));
            mixed.Add(new TextEncoding(Switch.Lowercase, 27));
            mixed.Add(new TextEncoding(Switch.Uppercase, 28));
            mixed.Add(new TextEncoding(Switch.PunctuationNextOnly, 29));

            TextMatrices.Insert(2, mixed);

            var punctuation = new TextEncodingTable();
            punctuation.Add(new TextEncoding(';', 0));
            punctuation.Add(new TextEncoding('<', 1));
            punctuation.Add(new TextEncoding('>', 2));
            punctuation.Add(new TextEncoding('@', 3));
            punctuation.Add(new TextEncoding('[', 4));
            punctuation.Add(new TextEncoding('\\', 5));
            punctuation.Add(new TextEncoding(']', 6));
            punctuation.Add(new TextEncoding('_', 7));
            punctuation.Add(new TextEncoding('`', 8));
            punctuation.Add(new TextEncoding('~', 9));
            punctuation.Add(new TextEncoding('!', 10));
            punctuation.Add(new TextEncoding('\r', 11));
            punctuation.Add(new TextEncoding('\t', 12));
            punctuation.Add(new TextEncoding(',', 13));
            punctuation.Add(new TextEncoding(':', 14));
            punctuation.Add(new TextEncoding('\n', 15));
            punctuation.Add(new TextEncoding('-', 16));
            punctuation.Add(new TextEncoding('.', 17));
            punctuation.Add(new TextEncoding('$', 18));
            punctuation.Add(new TextEncoding('/', 19));
            punctuation.Add(new TextEncoding('"', 20));
            punctuation.Add(new TextEncoding('|', 21));
            punctuation.Add(new TextEncoding('*', 22));
            punctuation.Add(new TextEncoding('(', 23));
            punctuation.Add(new TextEncoding(')', 24));
            punctuation.Add(new TextEncoding('?', 25));
            punctuation.Add(new TextEncoding('{', 26));
            punctuation.Add(new TextEncoding('}', 27));
            punctuation.Add(new TextEncoding('\'', 28));
            punctuation.Add(new TextEncoding(Switch.Uppercase, 29));

            TextMatrices.Insert(3, punctuation);
        }

        private void InitialiseErrorCodeFactors()
        {
            // Level 0
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[0].Add(27);
            ErrorCodeFactors[0].Add(917);

            // Level 1
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[1].Add(522);
            ErrorCodeFactors[1].Add(568);
            ErrorCodeFactors[1].Add(723);
            ErrorCodeFactors[1].Add(809);

            // Level 2
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[2].Add(237);
            ErrorCodeFactors[2].Add(308);
            ErrorCodeFactors[2].Add(436);
            ErrorCodeFactors[2].Add(284);
            ErrorCodeFactors[2].Add(646);
            ErrorCodeFactors[2].Add(653);
            ErrorCodeFactors[2].Add(428);
            ErrorCodeFactors[2].Add(379);

            // Level 3
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[3].Add(274);
            ErrorCodeFactors[3].Add(562);
            ErrorCodeFactors[3].Add(232);
            ErrorCodeFactors[3].Add(755);
            ErrorCodeFactors[3].Add(599);
            ErrorCodeFactors[3].Add(524);
            ErrorCodeFactors[3].Add(801);
            ErrorCodeFactors[3].Add(132);
            ErrorCodeFactors[3].Add(295);
            ErrorCodeFactors[3].Add(116);
            ErrorCodeFactors[3].Add(442);
            ErrorCodeFactors[3].Add(428);
            ErrorCodeFactors[3].Add(295);
            ErrorCodeFactors[3].Add(42);
            ErrorCodeFactors[3].Add(176);
            ErrorCodeFactors[3].Add(65);

            // Level 4
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[4].Add(361);
            ErrorCodeFactors[4].Add(575);
            ErrorCodeFactors[4].Add(922);
            ErrorCodeFactors[4].Add(525);
            ErrorCodeFactors[4].Add(176);
            ErrorCodeFactors[4].Add(586);
            ErrorCodeFactors[4].Add(640);
            ErrorCodeFactors[4].Add(321);
            ErrorCodeFactors[4].Add(536);
            ErrorCodeFactors[4].Add(742);
            ErrorCodeFactors[4].Add(677);
            ErrorCodeFactors[4].Add(742);
            ErrorCodeFactors[4].Add(687);
            ErrorCodeFactors[4].Add(284);
            ErrorCodeFactors[4].Add(193);
            ErrorCodeFactors[4].Add(517);
            ErrorCodeFactors[4].Add(273);
            ErrorCodeFactors[4].Add(494);
            ErrorCodeFactors[4].Add(263);
            ErrorCodeFactors[4].Add(147);
            ErrorCodeFactors[4].Add(593);
            ErrorCodeFactors[4].Add(800);
            ErrorCodeFactors[4].Add(571);
            ErrorCodeFactors[4].Add(320);
            ErrorCodeFactors[4].Add(803);
            ErrorCodeFactors[4].Add(133);
            ErrorCodeFactors[4].Add(231);
            ErrorCodeFactors[4].Add(390);
            ErrorCodeFactors[4].Add(685);
            ErrorCodeFactors[4].Add(330);
            ErrorCodeFactors[4].Add(63);
            ErrorCodeFactors[4].Add(410);

            // Level 5
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[5].Add(539);
            ErrorCodeFactors[5].Add(422);
            ErrorCodeFactors[5].Add(6);
            ErrorCodeFactors[5].Add(93);
            ErrorCodeFactors[5].Add(862);
            ErrorCodeFactors[5].Add(771);
            ErrorCodeFactors[5].Add(453);
            ErrorCodeFactors[5].Add(106);
            ErrorCodeFactors[5].Add(610);
            ErrorCodeFactors[5].Add(287);
            ErrorCodeFactors[5].Add(107);
            ErrorCodeFactors[5].Add(505);
            ErrorCodeFactors[5].Add(733);
            ErrorCodeFactors[5].Add(877);
            ErrorCodeFactors[5].Add(381);
            ErrorCodeFactors[5].Add(612);
            ErrorCodeFactors[5].Add(723);
            ErrorCodeFactors[5].Add(476);
            ErrorCodeFactors[5].Add(462);
            ErrorCodeFactors[5].Add(172);
            ErrorCodeFactors[5].Add(430);
            ErrorCodeFactors[5].Add(609);
            ErrorCodeFactors[5].Add(858);
            ErrorCodeFactors[5].Add(822);
            ErrorCodeFactors[5].Add(543);
            ErrorCodeFactors[5].Add(376);
            ErrorCodeFactors[5].Add(511);
            ErrorCodeFactors[5].Add(400);
            ErrorCodeFactors[5].Add(672);
            ErrorCodeFactors[5].Add(762);
            ErrorCodeFactors[5].Add(283);
            ErrorCodeFactors[5].Add(184);
            ErrorCodeFactors[5].Add(440);
            ErrorCodeFactors[5].Add(35);
            ErrorCodeFactors[5].Add(519);
            ErrorCodeFactors[5].Add(31);
            ErrorCodeFactors[5].Add(460);
            ErrorCodeFactors[5].Add(594);
            ErrorCodeFactors[5].Add(225);
            ErrorCodeFactors[5].Add(535);
            ErrorCodeFactors[5].Add(517);
            ErrorCodeFactors[5].Add(352);
            ErrorCodeFactors[5].Add(605);
            ErrorCodeFactors[5].Add(158);
            ErrorCodeFactors[5].Add(651);
            ErrorCodeFactors[5].Add(201);
            ErrorCodeFactors[5].Add(488);
            ErrorCodeFactors[5].Add(502);
            ErrorCodeFactors[5].Add(648);
            ErrorCodeFactors[5].Add(733);
            ErrorCodeFactors[5].Add(717);
            ErrorCodeFactors[5].Add(83);
            ErrorCodeFactors[5].Add(404);
            ErrorCodeFactors[5].Add(97);
            ErrorCodeFactors[5].Add(280);
            ErrorCodeFactors[5].Add(771);
            ErrorCodeFactors[5].Add(840);
            ErrorCodeFactors[5].Add(629);
            ErrorCodeFactors[5].Add(4);
            ErrorCodeFactors[5].Add(381);
            ErrorCodeFactors[5].Add(843);
            ErrorCodeFactors[5].Add(623);
            ErrorCodeFactors[5].Add(264);
            ErrorCodeFactors[5].Add(543);

            // Level 6
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[6].Add(521);
            ErrorCodeFactors[6].Add(310);
            ErrorCodeFactors[6].Add(864);
            ErrorCodeFactors[6].Add(547);
            ErrorCodeFactors[6].Add(858);
            ErrorCodeFactors[6].Add(580);
            ErrorCodeFactors[6].Add(296);
            ErrorCodeFactors[6].Add(379);
            ErrorCodeFactors[6].Add(53);
            ErrorCodeFactors[6].Add(779);
            ErrorCodeFactors[6].Add(897);
            ErrorCodeFactors[6].Add(444);
            ErrorCodeFactors[6].Add(400);
            ErrorCodeFactors[6].Add(925);
            ErrorCodeFactors[6].Add(749);
            ErrorCodeFactors[6].Add(415);
            ErrorCodeFactors[6].Add(822);
            ErrorCodeFactors[6].Add(93);
            ErrorCodeFactors[6].Add(217);
            ErrorCodeFactors[6].Add(208);
            ErrorCodeFactors[6].Add(928);
            ErrorCodeFactors[6].Add(244);
            ErrorCodeFactors[6].Add(583);
            ErrorCodeFactors[6].Add(620);
            ErrorCodeFactors[6].Add(246);
            ErrorCodeFactors[6].Add(148);
            ErrorCodeFactors[6].Add(447);
            ErrorCodeFactors[6].Add(631);
            ErrorCodeFactors[6].Add(292);
            ErrorCodeFactors[6].Add(908);
            ErrorCodeFactors[6].Add(490);
            ErrorCodeFactors[6].Add(704);
            ErrorCodeFactors[6].Add(516);
            ErrorCodeFactors[6].Add(258);
            ErrorCodeFactors[6].Add(457);
            ErrorCodeFactors[6].Add(907);
            ErrorCodeFactors[6].Add(594);
            ErrorCodeFactors[6].Add(723);
            ErrorCodeFactors[6].Add(674);
            ErrorCodeFactors[6].Add(292);
            ErrorCodeFactors[6].Add(272);
            ErrorCodeFactors[6].Add(96);
            ErrorCodeFactors[6].Add(684);
            ErrorCodeFactors[6].Add(432);
            ErrorCodeFactors[6].Add(686);
            ErrorCodeFactors[6].Add(606);
            ErrorCodeFactors[6].Add(860);
            ErrorCodeFactors[6].Add(569);
            ErrorCodeFactors[6].Add(193);
            ErrorCodeFactors[6].Add(219);
            ErrorCodeFactors[6].Add(129);
            ErrorCodeFactors[6].Add(186);
            ErrorCodeFactors[6].Add(236);
            ErrorCodeFactors[6].Add(287);
            ErrorCodeFactors[6].Add(192);
            ErrorCodeFactors[6].Add(775);
            ErrorCodeFactors[6].Add(278);
            ErrorCodeFactors[6].Add(173);
            ErrorCodeFactors[6].Add(40);
            ErrorCodeFactors[6].Add(379);
            ErrorCodeFactors[6].Add(712);
            ErrorCodeFactors[6].Add(463);
            ErrorCodeFactors[6].Add(646);
            ErrorCodeFactors[6].Add(776);
            ErrorCodeFactors[6].Add(171);
            ErrorCodeFactors[6].Add(491);
            ErrorCodeFactors[6].Add(297);
            ErrorCodeFactors[6].Add(763);
            ErrorCodeFactors[6].Add(156);
            ErrorCodeFactors[6].Add(732);
            ErrorCodeFactors[6].Add(95);
            ErrorCodeFactors[6].Add(270);
            ErrorCodeFactors[6].Add(447);
            ErrorCodeFactors[6].Add(90);
            ErrorCodeFactors[6].Add(507);
            ErrorCodeFactors[6].Add(48);
            ErrorCodeFactors[6].Add(228);
            ErrorCodeFactors[6].Add(821);
            ErrorCodeFactors[6].Add(808);
            ErrorCodeFactors[6].Add(898);
            ErrorCodeFactors[6].Add(784);
            ErrorCodeFactors[6].Add(663);
            ErrorCodeFactors[6].Add(627);
            ErrorCodeFactors[6].Add(378);
            ErrorCodeFactors[6].Add(382);
            ErrorCodeFactors[6].Add(262);
            ErrorCodeFactors[6].Add(380);
            ErrorCodeFactors[6].Add(602);
            ErrorCodeFactors[6].Add(754);
            ErrorCodeFactors[6].Add(336);
            ErrorCodeFactors[6].Add(89);
            ErrorCodeFactors[6].Add(614);
            ErrorCodeFactors[6].Add(87);
            ErrorCodeFactors[6].Add(432);
            ErrorCodeFactors[6].Add(670);
            ErrorCodeFactors[6].Add(616);
            ErrorCodeFactors[6].Add(157);
            ErrorCodeFactors[6].Add(374);
            ErrorCodeFactors[6].Add(242);
            ErrorCodeFactors[6].Add(726);
            ErrorCodeFactors[6].Add(600);
            ErrorCodeFactors[6].Add(269);
            ErrorCodeFactors[6].Add(375);
            ErrorCodeFactors[6].Add(898);
            ErrorCodeFactors[6].Add(845);
            ErrorCodeFactors[6].Add(454);
            ErrorCodeFactors[6].Add(354);
            ErrorCodeFactors[6].Add(130);
            ErrorCodeFactors[6].Add(814);
            ErrorCodeFactors[6].Add(587);
            ErrorCodeFactors[6].Add(804);
            ErrorCodeFactors[6].Add(34);
            ErrorCodeFactors[6].Add(211);
            ErrorCodeFactors[6].Add(330);
            ErrorCodeFactors[6].Add(539);
            ErrorCodeFactors[6].Add(297);
            ErrorCodeFactors[6].Add(827);
            ErrorCodeFactors[6].Add(865);
            ErrorCodeFactors[6].Add(37);
            ErrorCodeFactors[6].Add(517);
            ErrorCodeFactors[6].Add(834);
            ErrorCodeFactors[6].Add(315);
            ErrorCodeFactors[6].Add(550);
            ErrorCodeFactors[6].Add(86);
            ErrorCodeFactors[6].Add(801);
            ErrorCodeFactors[6].Add(4);
            ErrorCodeFactors[6].Add(108);
            ErrorCodeFactors[6].Add(539);

            // Level 7
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[7].Add(524);
            ErrorCodeFactors[7].Add(894);
            ErrorCodeFactors[7].Add(75);
            ErrorCodeFactors[7].Add(766);
            ErrorCodeFactors[7].Add(882);
            ErrorCodeFactors[7].Add(857);
            ErrorCodeFactors[7].Add(74);
            ErrorCodeFactors[7].Add(204);
            ErrorCodeFactors[7].Add(82);
            ErrorCodeFactors[7].Add(586);
            ErrorCodeFactors[7].Add(708);
            ErrorCodeFactors[7].Add(250);
            ErrorCodeFactors[7].Add(905);
            ErrorCodeFactors[7].Add(786);
            ErrorCodeFactors[7].Add(138);
            ErrorCodeFactors[7].Add(720);
            ErrorCodeFactors[7].Add(858);
            ErrorCodeFactors[7].Add(194);
            ErrorCodeFactors[7].Add(311);
            ErrorCodeFactors[7].Add(913);
            ErrorCodeFactors[7].Add(275);
            ErrorCodeFactors[7].Add(190);
            ErrorCodeFactors[7].Add(375);
            ErrorCodeFactors[7].Add(850);
            ErrorCodeFactors[7].Add(438);
            ErrorCodeFactors[7].Add(733);
            ErrorCodeFactors[7].Add(194);
            ErrorCodeFactors[7].Add(280);
            ErrorCodeFactors[7].Add(201);
            ErrorCodeFactors[7].Add(280);
            ErrorCodeFactors[7].Add(828);
            ErrorCodeFactors[7].Add(757);
            ErrorCodeFactors[7].Add(710);
            ErrorCodeFactors[7].Add(814);
            ErrorCodeFactors[7].Add(919);
            ErrorCodeFactors[7].Add(89);
            ErrorCodeFactors[7].Add(68);
            ErrorCodeFactors[7].Add(569);
            ErrorCodeFactors[7].Add(11);
            ErrorCodeFactors[7].Add(204);
            ErrorCodeFactors[7].Add(796);
            ErrorCodeFactors[7].Add(605);
            ErrorCodeFactors[7].Add(540);
            ErrorCodeFactors[7].Add(913);
            ErrorCodeFactors[7].Add(801);
            ErrorCodeFactors[7].Add(700);
            ErrorCodeFactors[7].Add(799);
            ErrorCodeFactors[7].Add(137);
            ErrorCodeFactors[7].Add(439);
            ErrorCodeFactors[7].Add(418);
            ErrorCodeFactors[7].Add(592);
            ErrorCodeFactors[7].Add(668);
            ErrorCodeFactors[7].Add(353);
            ErrorCodeFactors[7].Add(859);
            ErrorCodeFactors[7].Add(370);
            ErrorCodeFactors[7].Add(694);
            ErrorCodeFactors[7].Add(325);
            ErrorCodeFactors[7].Add(240);
            ErrorCodeFactors[7].Add(216);
            ErrorCodeFactors[7].Add(257);
            ErrorCodeFactors[7].Add(284);
            ErrorCodeFactors[7].Add(549);
            ErrorCodeFactors[7].Add(209);
            ErrorCodeFactors[7].Add(884);
            ErrorCodeFactors[7].Add(315);
            ErrorCodeFactors[7].Add(70);
            ErrorCodeFactors[7].Add(329);
            ErrorCodeFactors[7].Add(793);
            ErrorCodeFactors[7].Add(490);
            ErrorCodeFactors[7].Add(274);
            ErrorCodeFactors[7].Add(877);
            ErrorCodeFactors[7].Add(162);
            ErrorCodeFactors[7].Add(749);
            ErrorCodeFactors[7].Add(812);
            ErrorCodeFactors[7].Add(684);
            ErrorCodeFactors[7].Add(461);
            ErrorCodeFactors[7].Add(334);
            ErrorCodeFactors[7].Add(376);
            ErrorCodeFactors[7].Add(849);
            ErrorCodeFactors[7].Add(521);
            ErrorCodeFactors[7].Add(307);
            ErrorCodeFactors[7].Add(291);
            ErrorCodeFactors[7].Add(803);
            ErrorCodeFactors[7].Add(712);
            ErrorCodeFactors[7].Add(19);
            ErrorCodeFactors[7].Add(358);
            ErrorCodeFactors[7].Add(399);
            ErrorCodeFactors[7].Add(908);
            ErrorCodeFactors[7].Add(103);
            ErrorCodeFactors[7].Add(511);
            ErrorCodeFactors[7].Add(51);
            ErrorCodeFactors[7].Add(8);
            ErrorCodeFactors[7].Add(517);
            ErrorCodeFactors[7].Add(225);
            ErrorCodeFactors[7].Add(289);
            ErrorCodeFactors[7].Add(470);
            ErrorCodeFactors[7].Add(637);
            ErrorCodeFactors[7].Add(731);
            ErrorCodeFactors[7].Add(66);
            ErrorCodeFactors[7].Add(255);
            ErrorCodeFactors[7].Add(917);
            ErrorCodeFactors[7].Add(269);
            ErrorCodeFactors[7].Add(463);
            ErrorCodeFactors[7].Add(830);
            ErrorCodeFactors[7].Add(730);
            ErrorCodeFactors[7].Add(433);
            ErrorCodeFactors[7].Add(848);
            ErrorCodeFactors[7].Add(585);
            ErrorCodeFactors[7].Add(136);
            ErrorCodeFactors[7].Add(538);
            ErrorCodeFactors[7].Add(906);
            ErrorCodeFactors[7].Add(90);
            ErrorCodeFactors[7].Add(2);
            ErrorCodeFactors[7].Add(290);
            ErrorCodeFactors[7].Add(743);
            ErrorCodeFactors[7].Add(199);
            ErrorCodeFactors[7].Add(655);
            ErrorCodeFactors[7].Add(903);
            ErrorCodeFactors[7].Add(329);
            ErrorCodeFactors[7].Add(49);
            ErrorCodeFactors[7].Add(802);
            ErrorCodeFactors[7].Add(580);
            ErrorCodeFactors[7].Add(355);
            ErrorCodeFactors[7].Add(588);
            ErrorCodeFactors[7].Add(188);
            ErrorCodeFactors[7].Add(462);
            ErrorCodeFactors[7].Add(10);
            ErrorCodeFactors[7].Add(134);
            ErrorCodeFactors[7].Add(628);
            ErrorCodeFactors[7].Add(320);
            ErrorCodeFactors[7].Add(479);
            ErrorCodeFactors[7].Add(130);
            ErrorCodeFactors[7].Add(739);
            ErrorCodeFactors[7].Add(71);
            ErrorCodeFactors[7].Add(263);
            ErrorCodeFactors[7].Add(318);
            ErrorCodeFactors[7].Add(374);
            ErrorCodeFactors[7].Add(601);
            ErrorCodeFactors[7].Add(192);
            ErrorCodeFactors[7].Add(605);
            ErrorCodeFactors[7].Add(142);
            ErrorCodeFactors[7].Add(673);
            ErrorCodeFactors[7].Add(687);
            ErrorCodeFactors[7].Add(234);
            ErrorCodeFactors[7].Add(722);
            ErrorCodeFactors[7].Add(384);
            ErrorCodeFactors[7].Add(177);
            ErrorCodeFactors[7].Add(752);
            ErrorCodeFactors[7].Add(607);
            ErrorCodeFactors[7].Add(640);
            ErrorCodeFactors[7].Add(455);
            ErrorCodeFactors[7].Add(193);
            ErrorCodeFactors[7].Add(689);
            ErrorCodeFactors[7].Add(707);
            ErrorCodeFactors[7].Add(805);
            ErrorCodeFactors[7].Add(641);
            ErrorCodeFactors[7].Add(48);
            ErrorCodeFactors[7].Add(60);
            ErrorCodeFactors[7].Add(732);
            ErrorCodeFactors[7].Add(621);
            ErrorCodeFactors[7].Add(895);
            ErrorCodeFactors[7].Add(544);
            ErrorCodeFactors[7].Add(261);
            ErrorCodeFactors[7].Add(852);
            ErrorCodeFactors[7].Add(655);
            ErrorCodeFactors[7].Add(309);
            ErrorCodeFactors[7].Add(697);
            ErrorCodeFactors[7].Add(755);
            ErrorCodeFactors[7].Add(756);
            ErrorCodeFactors[7].Add(60);
            ErrorCodeFactors[7].Add(231);
            ErrorCodeFactors[7].Add(773);
            ErrorCodeFactors[7].Add(434);
            ErrorCodeFactors[7].Add(421);
            ErrorCodeFactors[7].Add(726);
            ErrorCodeFactors[7].Add(528);
            ErrorCodeFactors[7].Add(503);
            ErrorCodeFactors[7].Add(118);
            ErrorCodeFactors[7].Add(49);
            ErrorCodeFactors[7].Add(795);
            ErrorCodeFactors[7].Add(32);
            ErrorCodeFactors[7].Add(144);
            ErrorCodeFactors[7].Add(500);
            ErrorCodeFactors[7].Add(238);
            ErrorCodeFactors[7].Add(836);
            ErrorCodeFactors[7].Add(394);
            ErrorCodeFactors[7].Add(280);
            ErrorCodeFactors[7].Add(566);
            ErrorCodeFactors[7].Add(319);
            ErrorCodeFactors[7].Add(9);
            ErrorCodeFactors[7].Add(647);
            ErrorCodeFactors[7].Add(550);
            ErrorCodeFactors[7].Add(73);
            ErrorCodeFactors[7].Add(914);
            ErrorCodeFactors[7].Add(342);
            ErrorCodeFactors[7].Add(126);
            ErrorCodeFactors[7].Add(32);
            ErrorCodeFactors[7].Add(681);
            ErrorCodeFactors[7].Add(331);
            ErrorCodeFactors[7].Add(792);
            ErrorCodeFactors[7].Add(620);
            ErrorCodeFactors[7].Add(60);
            ErrorCodeFactors[7].Add(609);
            ErrorCodeFactors[7].Add(441);
            ErrorCodeFactors[7].Add(180);
            ErrorCodeFactors[7].Add(791);
            ErrorCodeFactors[7].Add(893);
            ErrorCodeFactors[7].Add(754);
            ErrorCodeFactors[7].Add(605);
            ErrorCodeFactors[7].Add(383);
            ErrorCodeFactors[7].Add(228);
            ErrorCodeFactors[7].Add(749);
            ErrorCodeFactors[7].Add(760);
            ErrorCodeFactors[7].Add(213);
            ErrorCodeFactors[7].Add(54);
            ErrorCodeFactors[7].Add(297);
            ErrorCodeFactors[7].Add(134);
            ErrorCodeFactors[7].Add(54);
            ErrorCodeFactors[7].Add(834);
            ErrorCodeFactors[7].Add(299);
            ErrorCodeFactors[7].Add(922);
            ErrorCodeFactors[7].Add(191);
            ErrorCodeFactors[7].Add(910);
            ErrorCodeFactors[7].Add(532);
            ErrorCodeFactors[7].Add(609);
            ErrorCodeFactors[7].Add(829);
            ErrorCodeFactors[7].Add(189);
            ErrorCodeFactors[7].Add(20);
            ErrorCodeFactors[7].Add(167);
            ErrorCodeFactors[7].Add(29);
            ErrorCodeFactors[7].Add(872);
            ErrorCodeFactors[7].Add(449);
            ErrorCodeFactors[7].Add(83);
            ErrorCodeFactors[7].Add(402);
            ErrorCodeFactors[7].Add(41);
            ErrorCodeFactors[7].Add(656);
            ErrorCodeFactors[7].Add(505);
            ErrorCodeFactors[7].Add(579);
            ErrorCodeFactors[7].Add(481);
            ErrorCodeFactors[7].Add(173);
            ErrorCodeFactors[7].Add(404);
            ErrorCodeFactors[7].Add(251);
            ErrorCodeFactors[7].Add(688);
            ErrorCodeFactors[7].Add(95);
            ErrorCodeFactors[7].Add(497);
            ErrorCodeFactors[7].Add(555);
            ErrorCodeFactors[7].Add(642);
            ErrorCodeFactors[7].Add(543);
            ErrorCodeFactors[7].Add(307);
            ErrorCodeFactors[7].Add(159);
            ErrorCodeFactors[7].Add(924);
            ErrorCodeFactors[7].Add(558);
            ErrorCodeFactors[7].Add(648);
            ErrorCodeFactors[7].Add(55);
            ErrorCodeFactors[7].Add(497);
            ErrorCodeFactors[7].Add(10);

            // Level 8
            ErrorCodeFactors.Add(new List<int>());
            ErrorCodeFactors[8].Add(352);
            ErrorCodeFactors[8].Add(77);
            ErrorCodeFactors[8].Add(373);
            ErrorCodeFactors[8].Add(504);
            ErrorCodeFactors[8].Add(35);
            ErrorCodeFactors[8].Add(599);
            ErrorCodeFactors[8].Add(428);
            ErrorCodeFactors[8].Add(207);
            ErrorCodeFactors[8].Add(409);
            ErrorCodeFactors[8].Add(574);
            ErrorCodeFactors[8].Add(118);
            ErrorCodeFactors[8].Add(498);
            ErrorCodeFactors[8].Add(285);
            ErrorCodeFactors[8].Add(380);
            ErrorCodeFactors[8].Add(350);
            ErrorCodeFactors[8].Add(492);
            ErrorCodeFactors[8].Add(197);
            ErrorCodeFactors[8].Add(265);
            ErrorCodeFactors[8].Add(920);
            ErrorCodeFactors[8].Add(155);
            ErrorCodeFactors[8].Add(914);
            ErrorCodeFactors[8].Add(299);
            ErrorCodeFactors[8].Add(229);
            ErrorCodeFactors[8].Add(643);
            ErrorCodeFactors[8].Add(294);
            ErrorCodeFactors[8].Add(871);
            ErrorCodeFactors[8].Add(306);
            ErrorCodeFactors[8].Add(88);
            ErrorCodeFactors[8].Add(87);
            ErrorCodeFactors[8].Add(193);
            ErrorCodeFactors[8].Add(352);
            ErrorCodeFactors[8].Add(781);
            ErrorCodeFactors[8].Add(846);
            ErrorCodeFactors[8].Add(75);
            ErrorCodeFactors[8].Add(327);
            ErrorCodeFactors[8].Add(520);
            ErrorCodeFactors[8].Add(435);
            ErrorCodeFactors[8].Add(543);
            ErrorCodeFactors[8].Add(203);
            ErrorCodeFactors[8].Add(666);
            ErrorCodeFactors[8].Add(249);
            ErrorCodeFactors[8].Add(346);
            ErrorCodeFactors[8].Add(781);
            ErrorCodeFactors[8].Add(621);
            ErrorCodeFactors[8].Add(640);
            ErrorCodeFactors[8].Add(268);
            ErrorCodeFactors[8].Add(794);
            ErrorCodeFactors[8].Add(534);
            ErrorCodeFactors[8].Add(539);
            ErrorCodeFactors[8].Add(781);
            ErrorCodeFactors[8].Add(408);
            ErrorCodeFactors[8].Add(390);
            ErrorCodeFactors[8].Add(644);
            ErrorCodeFactors[8].Add(102);
            ErrorCodeFactors[8].Add(476);
            ErrorCodeFactors[8].Add(499);
            ErrorCodeFactors[8].Add(290);
            ErrorCodeFactors[8].Add(632);
            ErrorCodeFactors[8].Add(545);
            ErrorCodeFactors[8].Add(37);
            ErrorCodeFactors[8].Add(858);
            ErrorCodeFactors[8].Add(916);
            ErrorCodeFactors[8].Add(552);
            ErrorCodeFactors[8].Add(41);
            ErrorCodeFactors[8].Add(542);
            ErrorCodeFactors[8].Add(289);
            ErrorCodeFactors[8].Add(122);
            ErrorCodeFactors[8].Add(272);
            ErrorCodeFactors[8].Add(383);
            ErrorCodeFactors[8].Add(800);
            ErrorCodeFactors[8].Add(485);
            ErrorCodeFactors[8].Add(98);
            ErrorCodeFactors[8].Add(752);
            ErrorCodeFactors[8].Add(472);
            ErrorCodeFactors[8].Add(761);
            ErrorCodeFactors[8].Add(107);
            ErrorCodeFactors[8].Add(784);
            ErrorCodeFactors[8].Add(860);
            ErrorCodeFactors[8].Add(658);
            ErrorCodeFactors[8].Add(741);
            ErrorCodeFactors[8].Add(290);
            ErrorCodeFactors[8].Add(204);
            ErrorCodeFactors[8].Add(681);
            ErrorCodeFactors[8].Add(407);
            ErrorCodeFactors[8].Add(855);
            ErrorCodeFactors[8].Add(85);
            ErrorCodeFactors[8].Add(99);
            ErrorCodeFactors[8].Add(62);
            ErrorCodeFactors[8].Add(482);
            ErrorCodeFactors[8].Add(180);
            ErrorCodeFactors[8].Add(20);
            ErrorCodeFactors[8].Add(297);
            ErrorCodeFactors[8].Add(451);
            ErrorCodeFactors[8].Add(593);
            ErrorCodeFactors[8].Add(913);
            ErrorCodeFactors[8].Add(142);
            ErrorCodeFactors[8].Add(808);
            ErrorCodeFactors[8].Add(684);
            ErrorCodeFactors[8].Add(287);
            ErrorCodeFactors[8].Add(536);
            ErrorCodeFactors[8].Add(561);
            ErrorCodeFactors[8].Add(76);
            ErrorCodeFactors[8].Add(653);
            ErrorCodeFactors[8].Add(899);
            ErrorCodeFactors[8].Add(729);
            ErrorCodeFactors[8].Add(567);
            ErrorCodeFactors[8].Add(744);
            ErrorCodeFactors[8].Add(390);
            ErrorCodeFactors[8].Add(513);
            ErrorCodeFactors[8].Add(192);
            ErrorCodeFactors[8].Add(516);
            ErrorCodeFactors[8].Add(258);
            ErrorCodeFactors[8].Add(240);
            ErrorCodeFactors[8].Add(518);
            ErrorCodeFactors[8].Add(794);
            ErrorCodeFactors[8].Add(395);
            ErrorCodeFactors[8].Add(768);
            ErrorCodeFactors[8].Add(848);
            ErrorCodeFactors[8].Add(51);
            ErrorCodeFactors[8].Add(610);
            ErrorCodeFactors[8].Add(384);
            ErrorCodeFactors[8].Add(168);
            ErrorCodeFactors[8].Add(190);
            ErrorCodeFactors[8].Add(826);
            ErrorCodeFactors[8].Add(328);
            ErrorCodeFactors[8].Add(596);
            ErrorCodeFactors[8].Add(786);
            ErrorCodeFactors[8].Add(303);
            ErrorCodeFactors[8].Add(570);
            ErrorCodeFactors[8].Add(381);
            ErrorCodeFactors[8].Add(415);
            ErrorCodeFactors[8].Add(641);
            ErrorCodeFactors[8].Add(156);
            ErrorCodeFactors[8].Add(237);
            ErrorCodeFactors[8].Add(151);
            ErrorCodeFactors[8].Add(429);
            ErrorCodeFactors[8].Add(531);
            ErrorCodeFactors[8].Add(207);
            ErrorCodeFactors[8].Add(676);
            ErrorCodeFactors[8].Add(710);
            ErrorCodeFactors[8].Add(89);
            ErrorCodeFactors[8].Add(168);
            ErrorCodeFactors[8].Add(304);
            ErrorCodeFactors[8].Add(402);
            ErrorCodeFactors[8].Add(40);
            ErrorCodeFactors[8].Add(708);
            ErrorCodeFactors[8].Add(575);
            ErrorCodeFactors[8].Add(162);
            ErrorCodeFactors[8].Add(864);
            ErrorCodeFactors[8].Add(229);
            ErrorCodeFactors[8].Add(65);
            ErrorCodeFactors[8].Add(861);
            ErrorCodeFactors[8].Add(841);
            ErrorCodeFactors[8].Add(512);
            ErrorCodeFactors[8].Add(164);
            ErrorCodeFactors[8].Add(477);
            ErrorCodeFactors[8].Add(221);
            ErrorCodeFactors[8].Add(92);
            ErrorCodeFactors[8].Add(358);
            ErrorCodeFactors[8].Add(785);
            ErrorCodeFactors[8].Add(288);
            ErrorCodeFactors[8].Add(357);
            ErrorCodeFactors[8].Add(850);
            ErrorCodeFactors[8].Add(836);
            ErrorCodeFactors[8].Add(827);
            ErrorCodeFactors[8].Add(736);
            ErrorCodeFactors[8].Add(707);
            ErrorCodeFactors[8].Add(94);
            ErrorCodeFactors[8].Add(8);
            ErrorCodeFactors[8].Add(494);
            ErrorCodeFactors[8].Add(114);
            ErrorCodeFactors[8].Add(521);
            ErrorCodeFactors[8].Add(2);
            ErrorCodeFactors[8].Add(499);
            ErrorCodeFactors[8].Add(851);
            ErrorCodeFactors[8].Add(543);
            ErrorCodeFactors[8].Add(152);
            ErrorCodeFactors[8].Add(729);
            ErrorCodeFactors[8].Add(771);
            ErrorCodeFactors[8].Add(95);
            ErrorCodeFactors[8].Add(248);
            ErrorCodeFactors[8].Add(361);
            ErrorCodeFactors[8].Add(578);
            ErrorCodeFactors[8].Add(323);
            ErrorCodeFactors[8].Add(856);
            ErrorCodeFactors[8].Add(797);
            ErrorCodeFactors[8].Add(289);
            ErrorCodeFactors[8].Add(51);
            ErrorCodeFactors[8].Add(684);
            ErrorCodeFactors[8].Add(466);
            ErrorCodeFactors[8].Add(533);
            ErrorCodeFactors[8].Add(820);
            ErrorCodeFactors[8].Add(669);
            ErrorCodeFactors[8].Add(45);
            ErrorCodeFactors[8].Add(902);
            ErrorCodeFactors[8].Add(452);
            ErrorCodeFactors[8].Add(167);
            ErrorCodeFactors[8].Add(342);
            ErrorCodeFactors[8].Add(244);
            ErrorCodeFactors[8].Add(173);
            ErrorCodeFactors[8].Add(35);
            ErrorCodeFactors[8].Add(463);
            ErrorCodeFactors[8].Add(651);
            ErrorCodeFactors[8].Add(51);
            ErrorCodeFactors[8].Add(699);
            ErrorCodeFactors[8].Add(591);
            ErrorCodeFactors[8].Add(452);
            ErrorCodeFactors[8].Add(578);
            ErrorCodeFactors[8].Add(37);
            ErrorCodeFactors[8].Add(124);
            ErrorCodeFactors[8].Add(298);
            ErrorCodeFactors[8].Add(332);
            ErrorCodeFactors[8].Add(552);
            ErrorCodeFactors[8].Add(43);
            ErrorCodeFactors[8].Add(427);
            ErrorCodeFactors[8].Add(119);
            ErrorCodeFactors[8].Add(662);
            ErrorCodeFactors[8].Add(777);
            ErrorCodeFactors[8].Add(475);
            ErrorCodeFactors[8].Add(850);
            ErrorCodeFactors[8].Add(764);
            ErrorCodeFactors[8].Add(364);
            ErrorCodeFactors[8].Add(578);
            ErrorCodeFactors[8].Add(911);
            ErrorCodeFactors[8].Add(283);
            ErrorCodeFactors[8].Add(711);
            ErrorCodeFactors[8].Add(472);
            ErrorCodeFactors[8].Add(420);
            ErrorCodeFactors[8].Add(245);
            ErrorCodeFactors[8].Add(288);
            ErrorCodeFactors[8].Add(594);
            ErrorCodeFactors[8].Add(394);
            ErrorCodeFactors[8].Add(511);
            ErrorCodeFactors[8].Add(327);
            ErrorCodeFactors[8].Add(589);
            ErrorCodeFactors[8].Add(777);
            ErrorCodeFactors[8].Add(699);
            ErrorCodeFactors[8].Add(688);
            ErrorCodeFactors[8].Add(43);
            ErrorCodeFactors[8].Add(408);
            ErrorCodeFactors[8].Add(842);
            ErrorCodeFactors[8].Add(383);
            ErrorCodeFactors[8].Add(721);
            ErrorCodeFactors[8].Add(521);
            ErrorCodeFactors[8].Add(560);
            ErrorCodeFactors[8].Add(644);
            ErrorCodeFactors[8].Add(714);
            ErrorCodeFactors[8].Add(559);
            ErrorCodeFactors[8].Add(62);
            ErrorCodeFactors[8].Add(145);
            ErrorCodeFactors[8].Add(873);
            ErrorCodeFactors[8].Add(663);
            ErrorCodeFactors[8].Add(713);
            ErrorCodeFactors[8].Add(159);
            ErrorCodeFactors[8].Add(672);
            ErrorCodeFactors[8].Add(729);
            ErrorCodeFactors[8].Add(624);
            ErrorCodeFactors[8].Add(59);
            ErrorCodeFactors[8].Add(193);
            ErrorCodeFactors[8].Add(417);
            ErrorCodeFactors[8].Add(158);
            ErrorCodeFactors[8].Add(209);
            ErrorCodeFactors[8].Add(563);
            ErrorCodeFactors[8].Add(564);
            ErrorCodeFactors[8].Add(343);
            ErrorCodeFactors[8].Add(693);
            ErrorCodeFactors[8].Add(109);
            ErrorCodeFactors[8].Add(608);
            ErrorCodeFactors[8].Add(563);
            ErrorCodeFactors[8].Add(365);
            ErrorCodeFactors[8].Add(181);
            ErrorCodeFactors[8].Add(772);
            ErrorCodeFactors[8].Add(677);
            ErrorCodeFactors[8].Add(310);
            ErrorCodeFactors[8].Add(248);
            ErrorCodeFactors[8].Add(353);
            ErrorCodeFactors[8].Add(708);
            ErrorCodeFactors[8].Add(410);
            ErrorCodeFactors[8].Add(579);
            ErrorCodeFactors[8].Add(870);
            ErrorCodeFactors[8].Add(617);
            ErrorCodeFactors[8].Add(841);
            ErrorCodeFactors[8].Add(632);
            ErrorCodeFactors[8].Add(860);
            ErrorCodeFactors[8].Add(289);
            ErrorCodeFactors[8].Add(536);
            ErrorCodeFactors[8].Add(35);
            ErrorCodeFactors[8].Add(777);
            ErrorCodeFactors[8].Add(618);
            ErrorCodeFactors[8].Add(586);
            ErrorCodeFactors[8].Add(424);
            ErrorCodeFactors[8].Add(833);
            ErrorCodeFactors[8].Add(77);
            ErrorCodeFactors[8].Add(597);
            ErrorCodeFactors[8].Add(346);
            ErrorCodeFactors[8].Add(269);
            ErrorCodeFactors[8].Add(757);
            ErrorCodeFactors[8].Add(632);
            ErrorCodeFactors[8].Add(695);
            ErrorCodeFactors[8].Add(751);
            ErrorCodeFactors[8].Add(331);
            ErrorCodeFactors[8].Add(247);
            ErrorCodeFactors[8].Add(184);
            ErrorCodeFactors[8].Add(45);
            ErrorCodeFactors[8].Add(787);
            ErrorCodeFactors[8].Add(680);
            ErrorCodeFactors[8].Add(18);
            ErrorCodeFactors[8].Add(66);
            ErrorCodeFactors[8].Add(407);
            ErrorCodeFactors[8].Add(369);
            ErrorCodeFactors[8].Add(54);
            ErrorCodeFactors[8].Add(492);
            ErrorCodeFactors[8].Add(228);
            ErrorCodeFactors[8].Add(613);
            ErrorCodeFactors[8].Add(830);
            ErrorCodeFactors[8].Add(922);
            ErrorCodeFactors[8].Add(437);
            ErrorCodeFactors[8].Add(519);
            ErrorCodeFactors[8].Add(644);
            ErrorCodeFactors[8].Add(905);
            ErrorCodeFactors[8].Add(789);
            ErrorCodeFactors[8].Add(420);
            ErrorCodeFactors[8].Add(305);
            ErrorCodeFactors[8].Add(441);
            ErrorCodeFactors[8].Add(207);
            ErrorCodeFactors[8].Add(300);
            ErrorCodeFactors[8].Add(892);
            ErrorCodeFactors[8].Add(827);
            ErrorCodeFactors[8].Add(141);
            ErrorCodeFactors[8].Add(537);
            ErrorCodeFactors[8].Add(381);
            ErrorCodeFactors[8].Add(662);
            ErrorCodeFactors[8].Add(513);
            ErrorCodeFactors[8].Add(56);
            ErrorCodeFactors[8].Add(252);
            ErrorCodeFactors[8].Add(341);
            ErrorCodeFactors[8].Add(242);
            ErrorCodeFactors[8].Add(797);
            ErrorCodeFactors[8].Add(838);
            ErrorCodeFactors[8].Add(837);
            ErrorCodeFactors[8].Add(720);
            ErrorCodeFactors[8].Add(224);
            ErrorCodeFactors[8].Add(307);
            ErrorCodeFactors[8].Add(631);
            ErrorCodeFactors[8].Add(61);
            ErrorCodeFactors[8].Add(87);
            ErrorCodeFactors[8].Add(560);
            ErrorCodeFactors[8].Add(310);
            ErrorCodeFactors[8].Add(756);
            ErrorCodeFactors[8].Add(665);
            ErrorCodeFactors[8].Add(397);
            ErrorCodeFactors[8].Add(808);
            ErrorCodeFactors[8].Add(851);
            ErrorCodeFactors[8].Add(309);
            ErrorCodeFactors[8].Add(473);
            ErrorCodeFactors[8].Add(795);
            ErrorCodeFactors[8].Add(378);
            ErrorCodeFactors[8].Add(31);
            ErrorCodeFactors[8].Add(647);
            ErrorCodeFactors[8].Add(915);
            ErrorCodeFactors[8].Add(459);
            ErrorCodeFactors[8].Add(806);
            ErrorCodeFactors[8].Add(590);
            ErrorCodeFactors[8].Add(731);
            ErrorCodeFactors[8].Add(425);
            ErrorCodeFactors[8].Add(216);
            ErrorCodeFactors[8].Add(548);
            ErrorCodeFactors[8].Add(249);
            ErrorCodeFactors[8].Add(321);
            ErrorCodeFactors[8].Add(881);
            ErrorCodeFactors[8].Add(699);
            ErrorCodeFactors[8].Add(535);
            ErrorCodeFactors[8].Add(673);
            ErrorCodeFactors[8].Add(782);
            ErrorCodeFactors[8].Add(210);
            ErrorCodeFactors[8].Add(815);
            ErrorCodeFactors[8].Add(905);
            ErrorCodeFactors[8].Add(303);
            ErrorCodeFactors[8].Add(843);
            ErrorCodeFactors[8].Add(922);
            ErrorCodeFactors[8].Add(281);
            ErrorCodeFactors[8].Add(73);
            ErrorCodeFactors[8].Add(469);
            ErrorCodeFactors[8].Add(791);
            ErrorCodeFactors[8].Add(660);
            ErrorCodeFactors[8].Add(162);
            ErrorCodeFactors[8].Add(498);
            ErrorCodeFactors[8].Add(308);
            ErrorCodeFactors[8].Add(155);
            ErrorCodeFactors[8].Add(422);
            ErrorCodeFactors[8].Add(907);
            ErrorCodeFactors[8].Add(817);
            ErrorCodeFactors[8].Add(187);
            ErrorCodeFactors[8].Add(62);
            ErrorCodeFactors[8].Add(16);
            ErrorCodeFactors[8].Add(425);
            ErrorCodeFactors[8].Add(535);
            ErrorCodeFactors[8].Add(336);
            ErrorCodeFactors[8].Add(286);
            ErrorCodeFactors[8].Add(437);
            ErrorCodeFactors[8].Add(375);
            ErrorCodeFactors[8].Add(273);
            ErrorCodeFactors[8].Add(610);
            ErrorCodeFactors[8].Add(296);
            ErrorCodeFactors[8].Add(183);
            ErrorCodeFactors[8].Add(923);
            ErrorCodeFactors[8].Add(116);
            ErrorCodeFactors[8].Add(667);
            ErrorCodeFactors[8].Add(751);
            ErrorCodeFactors[8].Add(353);
            ErrorCodeFactors[8].Add(62);
            ErrorCodeFactors[8].Add(366);
            ErrorCodeFactors[8].Add(691);
            ErrorCodeFactors[8].Add(379);
            ErrorCodeFactors[8].Add(687);
            ErrorCodeFactors[8].Add(842);
            ErrorCodeFactors[8].Add(37);
            ErrorCodeFactors[8].Add(357);
            ErrorCodeFactors[8].Add(720);
            ErrorCodeFactors[8].Add(742);
            ErrorCodeFactors[8].Add(330);
            ErrorCodeFactors[8].Add(5);
            ErrorCodeFactors[8].Add(39);
            ErrorCodeFactors[8].Add(923);
            ErrorCodeFactors[8].Add(311);
            ErrorCodeFactors[8].Add(424);
            ErrorCodeFactors[8].Add(242);
            ErrorCodeFactors[8].Add(749);
            ErrorCodeFactors[8].Add(321);
            ErrorCodeFactors[8].Add(54);
            ErrorCodeFactors[8].Add(669);
            ErrorCodeFactors[8].Add(316);
            ErrorCodeFactors[8].Add(342);
            ErrorCodeFactors[8].Add(299);
            ErrorCodeFactors[8].Add(534);
            ErrorCodeFactors[8].Add(105);
            ErrorCodeFactors[8].Add(667);
            ErrorCodeFactors[8].Add(488);
            ErrorCodeFactors[8].Add(640);
            ErrorCodeFactors[8].Add(672);
            ErrorCodeFactors[8].Add(576);
            ErrorCodeFactors[8].Add(540);
            ErrorCodeFactors[8].Add(316);
            ErrorCodeFactors[8].Add(486);
            ErrorCodeFactors[8].Add(721);
            ErrorCodeFactors[8].Add(610);
            ErrorCodeFactors[8].Add(46);
            ErrorCodeFactors[8].Add(656);
            ErrorCodeFactors[8].Add(447);
            ErrorCodeFactors[8].Add(171);
            ErrorCodeFactors[8].Add(616);
            ErrorCodeFactors[8].Add(464);
            ErrorCodeFactors[8].Add(190);
            ErrorCodeFactors[8].Add(531);
            ErrorCodeFactors[8].Add(297);
            ErrorCodeFactors[8].Add(321);
            ErrorCodeFactors[8].Add(762);
            ErrorCodeFactors[8].Add(752);
            ErrorCodeFactors[8].Add(533);
            ErrorCodeFactors[8].Add(175);
            ErrorCodeFactors[8].Add(134);
            ErrorCodeFactors[8].Add(14);
            ErrorCodeFactors[8].Add(381);
            ErrorCodeFactors[8].Add(433);
            ErrorCodeFactors[8].Add(717);
            ErrorCodeFactors[8].Add(45);
            ErrorCodeFactors[8].Add(111);
            ErrorCodeFactors[8].Add(20);
            ErrorCodeFactors[8].Add(596);
            ErrorCodeFactors[8].Add(284);
            ErrorCodeFactors[8].Add(736);
            ErrorCodeFactors[8].Add(138);
            ErrorCodeFactors[8].Add(646);
            ErrorCodeFactors[8].Add(411);
            ErrorCodeFactors[8].Add(877);
            ErrorCodeFactors[8].Add(669);
            ErrorCodeFactors[8].Add(141);
            ErrorCodeFactors[8].Add(919);
            ErrorCodeFactors[8].Add(45);
            ErrorCodeFactors[8].Add(780);
            ErrorCodeFactors[8].Add(407);
            ErrorCodeFactors[8].Add(164);
            ErrorCodeFactors[8].Add(332);
            ErrorCodeFactors[8].Add(899);
            ErrorCodeFactors[8].Add(165);
            ErrorCodeFactors[8].Add(726);
            ErrorCodeFactors[8].Add(600);
            ErrorCodeFactors[8].Add(325);
            ErrorCodeFactors[8].Add(498);
            ErrorCodeFactors[8].Add(655);
            ErrorCodeFactors[8].Add(357);
            ErrorCodeFactors[8].Add(752);
            ErrorCodeFactors[8].Add(768);
            ErrorCodeFactors[8].Add(223);
            ErrorCodeFactors[8].Add(849);
            ErrorCodeFactors[8].Add(647);
            ErrorCodeFactors[8].Add(63);
            ErrorCodeFactors[8].Add(310);
            ErrorCodeFactors[8].Add(863);
            ErrorCodeFactors[8].Add(251);
            ErrorCodeFactors[8].Add(366);
            ErrorCodeFactors[8].Add(304);
            ErrorCodeFactors[8].Add(282);
            ErrorCodeFactors[8].Add(738);
            ErrorCodeFactors[8].Add(675);
            ErrorCodeFactors[8].Add(410);
            ErrorCodeFactors[8].Add(389);
            ErrorCodeFactors[8].Add(244);
            ErrorCodeFactors[8].Add(31);
            ErrorCodeFactors[8].Add(121);
            ErrorCodeFactors[8].Add(303);
            ErrorCodeFactors[8].Add(263);
        }

        #endregion

        #region Helpers

        private bool IsUpper(char chr)
        {
            var idx = (int)SubTypes.Uppercase;
            return TextMatrices[idx].FirstOrDefault(enc => !enc.IsSwitch && enc.TextValue == chr) != null;
        }
        private bool IsLower(char chr)
        {
            var idx = (int)SubTypes.Lowercase;
            return TextMatrices[idx].FirstOrDefault(enc => !enc.IsSwitch && enc.TextValue == chr) != null;
        }
        private bool IsMixed(char chr)
        {
            var idx = (int)SubTypes.Mixed;
            return TextMatrices[idx].FirstOrDefault(enc => !enc.IsSwitch && enc.TextValue == chr) != null;
        }
        private bool IsPunctuation(char chr)
        {
            var idx = (int)SubTypes.Punctuation;
            return TextMatrices[idx].FirstOrDefault(enc => !enc.IsSwitch && enc.TextValue == chr) != null;
        }

        #endregion

        #region Row Indicators

        private CodeWord GetRowCodeWord(int rowNum, int rowCount, int cols, int level, BarcodeSide side)
        {
            var row = rowNum % 3;
            var x = side == BarcodeSide.Left ? GetLeftRowData(rowNum, rowCount, cols, level) : GetRightRowData(rowNum, rowCount, cols, level);
            var idx = 30 * (rowNum / 3) + x;
            return EncodingTables[row][idx];
        }
        private int GetLeftRowData(int row, int rows, int cols, int level)
        {
            var r = row % 3;
            switch (r)
            {
                case 0:
                    return ((rows - 1) / 3);
                case 1:
                    return level * 3 + (rows - 1) % 3;
                case 2:
                    return cols - 1;
                default:
                    throw new BarcodeException("Bad Left Row Stuff");
            }
        }
        private int GetRightRowData(int row, int rows, int cols, int level)
        {
            var r = row % 3;
            return r switch
            {
                0 => cols - 1,
                1 => ((rows - 1) / 3),
                2 => level * 3 + (rows - 1) % 3,
                _ => throw new BarcodeException("Bad Right Row Stuff"),
            };
        }

        #endregion

        #region Text Encoding

        private List<int> GetTextIndices(string text)
        {
            // get the indices of the relevant codewords for this encoding
            var indices = new List<int>();
            var encodings = GetTextEncodings(text);
            for (var i = 0; i < encodings.Count; i += 2)
            {
                indices.Add(encodings[i] * 30 + encodings[i + 1]);
            }
            // reverse order
            //indices.Reverse()
            return indices;
        }

        private List<int> GetTextEncodings(string text)
        {
            var currentSubType = SubTypes.Uppercase;
            var encodings = new List<int>();
            for (var i = 0; i < text.Length; i++)
            {
                var chr = text[i];
                currentSubType = currentSubType switch
                {
                    SubTypes.Uppercase => DoTextEncodingForUpper(text, i, ref encodings),
                    SubTypes.Lowercase => DoTextEncodingForLower(text, i, ref encodings),
                    SubTypes.Mixed => DoTextEncodingForMixed(text, i, ref encodings),
                    SubTypes.Punctuation => DoTextEncodingForPunctuation(text, i, ref encodings),
                    _ => throw new BarcodeException("Invalid SubType"),
                };
            }
            // pad encodings
            if (encodings.Count % 2 != 0)
            {
                encodings.Add((int)Switch.PunctuationNextOnly);
            }

            return encodings;
        }

        private SubTypes DoTextEncodingForUpper(string text, int idx, ref List<int> encodings)
        {
            var chr = text[idx];
            var mi = -1;
            var st = SubTypes.Uppercase;

            if (IsUpper(chr))
            {
                mi = (int)SubTypes.Uppercase;
            }
            else if (IsLower(chr))
            {
                mi = (int)SubTypes.Lowercase;
                encodings.Add((int)Switch.Lowercase);
                st = SubTypes.Lowercase;
            }
            else if (IsPunctuation(chr))
            {
                mi = (int)SubTypes.Punctuation;
                encodings.Add((int)Switch.PunctuationNextOnly);
            }
            else if (IsMixed(chr))
            {
                mi = (int)SubTypes.Mixed;
                encodings.Add((int)Switch.Mixed);
                st = SubTypes.Mixed;
            }
            else
            {
                // we need to switch to numeric only or bytes, but for now...
                throw new ArgumentException("Invalid character");
            }
            encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == chr).Encoding);
            return st;
        }

        private SubTypes DoTextEncodingForLower(string text, int idx, ref List<int> encodings)
        {
            var chr = text[idx];
            var mi = -1;
            var st = SubTypes.Lowercase;

            if (IsLower(chr))
            {
                mi = (int)SubTypes.Lowercase;
            }
            else if (IsUpper(chr))
            {
                mi = (int)SubTypes.Uppercase;
                encodings.Add((int)Switch.UppercaseNextOnly);
            }
            else if (IsPunctuation(chr))
            {
                mi = (int)SubTypes.Punctuation;
                encodings.Add((int)Switch.PunctuationNextOnly);
            }
            else if (IsMixed(chr))
            {
                mi = (int)SubTypes.Mixed;
                encodings.Add((int)Switch.Mixed);
                st = SubTypes.Mixed;
            }
            else
            {
                // we need to switch to numeric only or bytes, but for now...
                throw new ArgumentException("Invalid character");
            }
            encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == chr).Encoding);
            return st;
        }

        private SubTypes DoTextEncodingForMixed(string text, int idx, ref List<int> encodings)
        {
            var chr = text[idx];
            var mi = -1;
            var st = SubTypes.Mixed;

            if (IsMixed(chr))
            {
                mi = (int)SubTypes.Mixed;
            }
            else if (IsUpper(chr))
            {
                mi = (int)SubTypes.Uppercase;
                encodings.Add((int)Switch.Mixed); // Upper's switch is in the 'Mixed' position in the Mixed SubType. TODO: Cleanup
                st = SubTypes.Uppercase;
            }
            else if (IsLower(chr))
            {
                mi = (int)SubTypes.Lowercase;
                encodings.Add((int)Switch.Lowercase);
                st = SubTypes.Lowercase;
            }
            else if (IsPunctuation(chr))
            {
                mi = (int)SubTypes.Punctuation;
                // peek
                if (idx < text.Length - 1)
                {
                    var next = text[idx + 1];
                    if (IsPunctuation(next))
                    {
                        encodings.Add(25); // Punctuation's switch is in slot '25' in the Mixed AubType. TODO: Cleanup
                        st = SubTypes.Punctuation;
                    }
                    else
                    {
                        encodings.Add((int)Switch.PunctuationNextOnly);
                    }
                }
                else
                {
                    encodings.Add((int)Switch.PunctuationNextOnly);
                }
            }
            else
            {
                // we need to switch to numeric only or bytes, but for now...
                throw new ArgumentException("Invalid character");
            }
            encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == chr).Encoding);
            return st;
        }

        private SubTypes DoTextEncodingForPunctuation(string text, int idx, ref List<int> encodings)
        {
            var chr = text[idx];
            var mi = -1;
            var st = SubTypes.Punctuation;

            //WVN: Switch?
            if (IsPunctuation(chr))
            {
                mi = (int)SubTypes.Punctuation;
            }
            else if (IsUpper(chr))
            {
                mi = (int)SubTypes.Uppercase;
                encodings.Add((int)Switch.Uppercase);
                st = SubTypes.Uppercase;
            }
            else if (IsLower(chr))
            {
                mi = (int)SubTypes.Lowercase;
                encodings.Add((int)Switch.Uppercase);
                encodings.Add((int)Switch.Lowercase);
                st = SubTypes.Lowercase;
            }
            else if (IsMixed(chr))
            {
                mi = (int)SubTypes.Mixed;
                encodings.Add((int)Switch.Uppercase);
                encodings.Add((int)Switch.Mixed);
                st = SubTypes.Mixed;
            }
            else
            {
                // we need to switch to numeric only or bytes, but for now...
                throw new ArgumentException("Invalid character");
            }

            encodings.Add(TextMatrices[mi].FirstOrDefault(enc => enc.TextValue == chr).Encoding);
            return st;
        }

        #endregion

        #region Byte Encoding

        private List<int> GetByteIndices(byte[] data)
        {
            var indices = new List<int>();

            // byte switch
            if (data.Length % 6 == 0)
            {
                indices.Add(924);
            }
            else
            {
                indices.Add(901);
            }

            for (var b = 0; b < data.Length; b += 6)
            {
                var size = Math.Min(6, data.Length - b);
                var tmp = new byte[size];
                Array.ConstrainedCopy(data, b, tmp, 0, size);

                if (size != 6)
                {
                    indices.AddRange(tmp.Select(i => (int)i));
                    break;
                }

                var lrange = Enumerable.Range(0, size).Select(i => i).ToList();
                var value = lrange.Sum(idx => tmp[idx] * Math.Pow(256, size - (idx + 1)));

                var work = new List<int>();
                Enumerable.Range(0, size).Aggregate(value, (val, idx) => { work.Add((int)(val % 900)); val /= 900; return val; });
                work.Reverse();
                work.Remove(0);
                indices.AddRange(work);
            }

            return indices;
        }

        #endregion

        #region Number Encoding

        // NOTE: This is not complete. Proper number encoding works on HUGE numbers (groups of 44 digits) which I haven't implemented yet
        // this will fail if the number is bigger than 18 digits long, and might even fail on 18 digits
        private static List<int> GetNumberIndices(long number)
        {
            var indices = new List<int>
            {
                902
            };

            // long is max 19 digits, so no need to split into groups of 44
            var digits = number.ToString(System.Globalization.CultureInfo.InvariantCulture).Length;
            var worknum = number + (long)Math.Pow(10, digits);
            var work = new List<int>();
            while (worknum != 0)
            {
                work.Add((int)(worknum % 900));
                worknum /= 900;
            }
            work.Reverse();
            indices.AddRange(work);
            return indices;
        }

        #endregion

        #region Error Correction

        private List<int> GetErrorCorrectionIndices(List<int> data, int level)
        {
            if (level < 0 || level > 8)
            {
                throw new ArgumentException("level must be between 0 and 8, or -1 for auto");
            }

            var indices = new List<int>();
            var correctionCodeWords = (int)Math.Pow(2, level + 1);
            var correctionFactors = ErrorCodeFactors[level];

            // initialise
            for (var i = 0; i < correctionCodeWords; i++)
            {
                indices.Add(0);
            }

            for (var i = 0; i < data.Count; i++)
            {
                var t = (data[i] + indices[correctionCodeWords - 1]) % 929;
                for (var j = correctionCodeWords - 1; j >= 0; j--)
                {
                    indices[j] = j == 0
                        ? (929 - (t * correctionFactors[j]) % 929) % 929
                        : (indices[j - 1] + 929 - (t * correctionFactors[j]) % 929) % 929;
                }
            }
            for (var j = 0; j < correctionCodeWords; j++)
            {
                if (indices[j] != 0)
                {
                    indices[j] = 929 - indices[j];
                }
            }
            // reverse order
            indices.Reverse();
            return indices;
        }

        #endregion

        #region Automatic Settings

        private int RecommendedErrorCorrectionLevel(int dataCount)
        {
            //WVN: switch
            if (dataCount > 320)
            {
                return 5;
            }

            if (dataCount > 160)
            {
                return 4;
            }

            if (dataCount > 40)
            {
                return 3;
            }

            return 2;
        }

        private static int[] BestFitBlocks(int count)
        {
            // A bit broken when plenty of blocks are required

            //// 1:3 col:row
            //var ratio = new int[2]
            //var rt = Math.Sqrt(count / 3f) + 0.3f
            //ratio[0] = (int)(rt * 1)
            //ratio[1] = (int)(rt * 3)
            //return ratio
            var cols = (int)Math.Sqrt(count);
            var rows = (int)Math.Ceiling(count / (float)cols);
            return new int[] { cols, rows };
        }

        private static int CalculateDataBlocks(int dataCount, int level)
        {
            var blocks = dataCount + 1; // add 1 block for data count codeword
            blocks += (int)Math.Pow(2, level + 1);
            return blocks;
        }

        #endregion

        #region Builders

        private List<CodeWord> GetCodewords(List<int> indices, int cols)
        {
            var row = new IndexWrapper(3); // loop through encoding tables
            var col = new IndexWrapper(cols);
            var result = new List<CodeWord>();
            foreach (var idx in indices)
            {
                result.Add(EncodingTables[row.Current][idx]);
                if (col.Increment() == 0)
                {
                    row.Increment();
                }
            }
            return result;
        }

        private Bitmap DrawBarcode(List<int> data, Pdf417Settings settings)
        {
            // TODO: Cleanup

            var rows = settings.Rows;
            var cols = settings.Columns;
            var eclevel = settings.ErrorCorrectionLevel;
            ModuleWidth = settings.ModuleWidth;

            if (eclevel == -1)
            {
                eclevel = RecommendedErrorCorrectionLevel(data.Count);
            }

            var blocks = CalculateDataBlocks(data.Count, eclevel);
            if (rows == 0 || cols == 0)
            {
                var ratio = BestFitBlocks(blocks);
                cols = ratio[0];
                rows = ratio[1];
            }

            if (rows > 90)
            {
                throw new ArgumentException("Too many rows. Maximum = 90");
            }

            if (rows < 3)
            {
                throw new ArgumentException("Not enough rows. Minimum = 3");
            }

            if (cols > 30)
            {
                throw new ArgumentException("Too many columns. Maximum = 30");
            }

            if (cols < 1)
            {
                throw new ArgumentException("Not enough columns. Minimum = 1");
            }

            // check if there's currently enough space for the data
            if ((rows * cols) < blocks)
            {
                throw new BarcodeException("Not enough rows and columns to hold the data. Blocks required: " + blocks);
            }

            // padding
            var padding = new List<int>();
            var dataPads = cols - (blocks % cols);
            var min = blocks % cols == 0 ? 0 : 1;
            dataPads += (rows - ((blocks / cols) + min)) * cols;
            if (dataPads != cols)
            {
                padding.AddRange(Enumerable.Range(0, dataPads).Select(i => 900).ToList());
                blocks += dataPads;
            }

            var indices = new List<int>(data);
            indices.AddRange(padding);
            indices.Insert(0, indices.Count + 1);

            // add error correction
            var errorIndices = GetErrorCorrectionIndices(indices, eclevel);
            indices.AddRange(errorIndices);

            // map indices to codewords
            var codeWords = GetCodewords(indices, cols);

            var w = ModuleWidth; // module width
            var h = w * 3; // module height
            var q = w * 2; // quite zone
            var width = ((17 * cols) + 69) * w + 2 * q;
            var height = rows * h + 2 * q;
            var bmp = new Bitmap(width, height);

            // set resolution
            bmp.SetResolution(settings.HorizontalDPI, settings.VerticalDPI);

            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.InterpolationMode = InterpolationMode.High;
                gfx.Clear(Color.White);

                var x = q;
                var y = q;
                var d = 0;

                for (var row = 0; row < rows; row++)
                {
                    // start
                    gfx.DrawCodeWord(StartPattern, ref x, ref y, ref w, ref h);
                    // left row info
                    gfx.DrawCodeWord(GetRowCodeWord(row, rows, cols, eclevel, BarcodeSide.Left), ref x, ref y, ref w, ref h);
                    // data
                    var dmax = d + cols;
                    for (; d < dmax; d++)
                    {
                        gfx.DrawCodeWord(codeWords[d], ref x, ref y, ref w, ref h);
                    }
                    // right row info
                    gfx.DrawCodeWord(GetRowCodeWord(row, rows, cols, eclevel, BarcodeSide.Right), ref x, ref y, ref w, ref h);
                    // end
                    gfx.DrawCodeWord(EndPattern, ref x, ref y, ref w, ref h);
                    // reset
                    x = q;     // CR
                    y = y + h; // LF
                }
            }
            return bmp;
        }

        #endregion

        #endregion

        #region Public Methods

        public Bitmap Encode(string text) => Encode(text, new Pdf417Settings());

        public Bitmap Encode(string text, BarcodeSettings settings) => DrawBarcode(GetTextIndices(text), settings as Pdf417Settings);

        public Bitmap Encode(byte[] data) => Encode(data, new Pdf417Settings());

        public Bitmap Encode(byte[] data, BarcodeSettings settings) => DrawBarcode(GetByteIndices(data), settings as Pdf417Settings);

        public Bitmap Encode(long number) => Encode(number, new Pdf417Settings());

        public Bitmap Encode(long number, BarcodeSettings settings) => DrawBarcode(GetNumberIndices(number), settings as Pdf417Settings);

        public byte[] EncodeToBytes(string text) => EncodeToBytes(text, new Pdf417Settings());

        public byte[] EncodeToBytes(string text, BarcodeSettings settings) => DrawBarcode(GetTextIndices(text), settings as Pdf417Settings).ToByteArray(settings.ImageFormat);

        public byte[] EncodeToBytes(byte[] data) => EncodeToBytes(data, new Pdf417Settings());

        public byte[] EncodeToBytes(byte[] data, BarcodeSettings settings) => DrawBarcode(GetByteIndices(data), settings as Pdf417Settings).ToByteArray(settings.ImageFormat);

        public byte[] EncodeToBytes(long number) => EncodeToBytes(number, new Pdf417Settings());

        public byte[] EncodeToBytes(long number, BarcodeSettings settings) => DrawBarcode(GetNumberIndices(number), settings as Pdf417Settings).ToByteArray(settings.ImageFormat);

        #endregion
    }
}
