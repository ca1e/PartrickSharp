namespace PartrickSharp
{
    internal class RandKey
	{
		const int STATE_SIZE = 4;
		const int NUM_ROUNDS = 4;

		static uint[] generateState(byte[] data)
		{
			// default BE
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
					var idx4 = KeyTables.COURSE[idx1] >> (int)idx3;
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
