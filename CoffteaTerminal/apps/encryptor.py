import codecs
import base64
from itertools import cycle
class Decoder():
    def __init__(self) -> None:
        self.name = "@:\\Cofftea\\CITcrypter\\>"
        self.key = "strongencryption".encode('utf8')
        self.crypts = []

    def isascii(self, data: bytes) -> bool:
        for c in data:
            if not(32 <= ord(c) <= 127):
                return False
        return True

    def encryptATbash(self, m):
        ct = {65 : 90, 66 : 89, 67 : 88, 68 : 87, 69 : 86, 70 : 85, 71 : 84, 72 : 83, 73 : 82, 74 : 81, 75 : 80, 76 : 79, 77 : 78, 78 : 77, 79 : 76, 80 : 75, 81 : 74, 82 : 73, 83 : 72, 84 : 71, 85 : 70, 86 : 69, 87 : 68, 88 : 67, 89 : 66, 90 : 65}
        encrypted = ''
        for l in m:
            if (l in ct):
                encrypted += chr(ct[l])
            else:
                encrypted += chr(l)
        return encrypted

    def is_base64_char(self, c):
        return 'A' <= c <= 'Z' or 'a' <= c <= 'z' or '0' <= c <= '9' or c == '+' or c == '/' or c == '='

    def string_is_base64(self, s):
        if s.find("=") < len(s) - 2 and s.find("=") != -1:
            return False
        if s.find("=") == len(s) - 2 and s[-1] != "=" and s.find("=") != -1:
            return False
        if s[-1] == '=':
            if s[-2] == '=' and len(s) % 4 == 0:
                return True
            else:
                if len(s) % 4 == 0:
                    return True
        else:
            if len(s) % 4 == 0:
                return True
        return False

    def decrypt(self, data: str) -> str:
        
        temp = data.replace('|', "").replace('_', "").replace("@", "").replace("!", "").replace(":", "").replace("?", "")
        #print(temp)
        
        temp = self.encryptATbash(temp.encode('utf8'))[::-1]
        
        #get all possible substrings
        possible = []
        for i in range(len(temp)):
            for j in range(len(temp) - i):
                sub = temp[i:i+j+1]
                if len(sub) > 4 and self.string_is_base64(sub) and all([self.is_base64_char(i) for i in sub]):
                    sub = base64.b64decode(sub.encode('utf-8'))
                    if not self.isascii([chr(o) for o in [a ^ b for a, b in zip(sub, cycle(self.key))]]):
                        continue
                    sub = "".join([chr(o) for o in [a ^ b for a, b in zip(sub, cycle(self.key))]])
                    possible.append(sub)
                    if j % 100 == 0:
                        print(i, j)
                else:
                    continue
        return possible

    def find_cipher(self, data_list):
        for t_data in data_list:
            data = t_data[1]
            data = codecs.decode(data, 'ascii', errors='ignore')
            crypts = []
            clean_crypts = []
            temp = ""
            for i in data:
                if self.isascii(i):
                    temp += i
                else:
                    if len(temp) > 6:
                        crypts.append(temp)
                    temp = ""
            for i in crypts: 
                base = self.decrypt(i)
                if (len(base) > 0):
                    for j in base:
                        if len(j) > 0:
                            clean_crypts.append((t_data[0], j))
            self.crypts += list(set(clean_crypts))
        return self.crypts

    def run(self, data: bytes, *args) -> str:
        return self.find_cipher(data)