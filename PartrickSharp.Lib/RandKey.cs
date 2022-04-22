namespace PartrickSharp
{
    internal class RandKey
	{
		const int STATE_SIZE = 4;
		const int NUM_ROUNDS = 4;

		static uint[] course_key_table =
		{
		0x7AB1C9D2, 0xCA750936, 0x3003E59C, 0xF261014B,
		0x2E25160A, 0xED614811, 0xF1AC6240, 0xD59272CD,
		0xF38549BF, 0x6CF5B327, 0xDA4DB82A, 0x820C435A,
		0xC95609BA, 0x19BE08B0, 0x738E2B81, 0xED3C349A,
		0x45275D1,  0xE0A73635, 0x1DEBF4DA, 0x9924B0DE,
		0x6A1FC367, 0x71970467, 0xFC55ABEB, 0x368D7489,
		0xCC97D1D,  0x17CC441E, 0x3528D152, 0xD0129B53,
		0xE12A69E9, 0x13D1BDB7, 0x32EAA9ED, 0x42F41D1B,
		0xAEA5F51F, 0x42C5D23C, 0x7CC742ED, 0x723BA5F9,
		0xDE5B99E3, 0x2C0055A4, 0xC38807B4, 0x4C099B61,
		0xC4E4568E, 0x8C29C901, 0xE13B34AC, 0xE7C3F212,
		0xB67EF941, 0x8038965,  0x8AFD1E6A, 0x8E5341A3,
		0xA4C61107, 0xFBAF1418, 0x9B05EF64, 0x3C91734E,
		0x82EC6646, 0xFB19F33E, 0x3BDE6FE2, 0x17A84CCA,
		0xCCDF0CE9, 0x50E4135C, 0xFF2658B2, 0x3780F156,
		0x7D8F5D68, 0x517CBED1, 0x1FCDDF0D, 0x77A58C94
		};

		static uint[] generateState(byte[] data)
		{
			var in1 = BitConverter.ToUInt32(data, 0);
			var in2 = BitConverter.ToUInt32(data, 4);
			var in3 = BitConverter.ToUInt32(data, 8);
			var in4 = BitConverter.ToUInt32(data, 12);

			var cond = (in1 | in2 | in3 | in4) != 0;

			return new uint[]{
			cond ? in1: 1,
			cond ? in2: 0x6C078967,
			cond ? in3: 0x714ACB41,
			cond ? in4: 0x48077044 };
		}

		static uint generateRand(ref uint[] rand_state)
		{
			uint n = rand_state[0] ^ rand_state[0] << 11;
			rand_state[0] = rand_state[1];
			n ^= n >> 8 ^ rand_state[3] ^ rand_state[3] >> 19;
			rand_state[1] = rand_state[2];
			rand_state[2] = rand_state[3];
			rand_state[3] = n;
			return n;
		}

		static uint[] generateKeyState(ref uint[] rand)
		{
			uint[] outKey = new uint[4] { 0, 0, 0, 0 };
			for (int i = 0; i < STATE_SIZE; i++)
			{
				for (int j = 0; j < NUM_ROUNDS; j++)
				{
					outKey[i] <<= 8;
					var idx1 = generateRand(ref rand) >> 26;
					var idx2 = generateRand(ref rand) >> 27;
					var idx3 = idx2 & 24;
					var idx4 = course_key_table[idx1] >> (int)idx3;
					outKey[i] |= idx4 & 0xFF;
				}
			}
			return outKey;
		}

		static byte[] generateKey(uint[] keyState)
		{
			return BitConverter.GetBytes(keyState[0])
				.Concat(BitConverter.GetBytes(keyState[1]))
				.Concat(BitConverter.GetBytes(keyState[2]))
				.Concat(BitConverter.GetBytes(keyState[3])).ToArray();
		}

		public static byte[] GetRandKey(byte[] data, out byte[]cmacKey)
        {
			var randState = generateState(data);
			var keyState = generateKeyState(ref randState);
			var cmacKeyState = generateKeyState(ref randState);
			cmacKey = generateKey(cmacKeyState);

			return generateKey(keyState);
		}
	}
}
