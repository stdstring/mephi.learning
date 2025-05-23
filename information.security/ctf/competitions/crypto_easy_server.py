from Crypto.Cipher import AES
from Crypto.Util.Padding import pad
from os import urandom

from flag import FLAG   # flag.py is on server


print("Can you recover flag?")
print(f"flag length: {len(FLAG)}")

key = urandom(32)
cipher = AES.new(key, AES.MODE_ECB)

while True:
    try:
        data = bytes.fromhex(input())
    except:
        print("Bad format")
        continue
    ciphertext = cipher.encrypt(pad(data + FLAG, 16))
    print(ciphertext.hex())
