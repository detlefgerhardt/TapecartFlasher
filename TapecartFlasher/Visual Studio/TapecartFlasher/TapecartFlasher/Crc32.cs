using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapecartFlasher {

    class Crc32 {

        public UInt32 _crc;

        public UInt32 Crc {
            get {
                return Finalize(_crc);
            }
        }

		public Crc32() {
            _crc = 0xffffffff;
		}

        public void Update(byte[] data, int offset, int length) {
            int i;
            bool bit;
            byte c;

            int pos = 0;
            for (int l=0; l<length;  l++) {
                c = data[offset + l];
                for ( i = 0x01; ( i & 0xff ) != 0; i <<= 1 ) {
                    bit = ( _crc & 0x80000000 ) != 0;
                    if ( ( c & i ) != 0 )
                        bit = !bit;
                    _crc <<= 1;
                    if ( bit )
                        _crc ^= 0x04c11db7;
                }
                _crc &= 0xffffffff;
            }
            _crc &= 0xffffffff;
        }

        private UInt32 Finalize(UInt32 crc)
        {
            return Reflect(crc, 32) ^ 0xffffffff;
        }

        private UInt32 Reflect(UInt32 data, int dataLen)
        {
            int i;
            UInt32 ret;

            ret = data & 0x01;
            for (i = 1; i < dataLen; i++)
            {
                data >>= 1;
                ret = (ret << 1) | (data & 0x01);
            }
            return ret;
        } 
	}
}
