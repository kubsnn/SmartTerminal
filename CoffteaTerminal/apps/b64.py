import codecs
import base64

class DecodeBase():
    def __init__(self):
        self.name = "@:\\Cofftea\\Base64\\>"
        self.bases = []

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

    def isascii(self, s):
        for c in s:
            if not(32 <= ord(c) <= 127):
                return False
        return True

    #find all substrings that can be base64 encoded
    def find_base64(self, data_list):
        for t_data in data_list:
            data = t_data[1]
            data = codecs.decode(data, 'ascii', errors='ignore').replace("\n", "").replace("\00", "")
            #print(data)
            bases = []
            clean_bases = []
            temp = ""
            for i in data:
                if self.is_base64_char(i):
                    temp += i
                else:
                    if len(temp) > 6:
                        bases.append(temp)
                    temp = ""
            for i in bases: 
                if len(i) > 1000:
                    continue
                if self.string_is_base64(i):
                    try:
                        b = base64.b64decode(i).decode().replace("\n", "")
                        if not self.isascii(b):
                            continue
                        clean_bases.append((t_data[0], b))
                    except Exception as e:
                        pass
            self.bases += clean_bases
            self.bases = list(set(self.bases))
        return self.bases
    
    def run(self, data, *args):
        return self.find_base64(data)