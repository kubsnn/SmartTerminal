class Decoder():
    def __init__(self) -> None:
        self.name = "@:\\Cofftea\\CITsimple\\>"

    def isascii(self, data: bytes) -> bool:
        for c in data:
            if not(32 <= c <= 127):
                return False
        return True

    def find_CITS(self, data_list):
        cits = []
        for t_data in data_list:
            data = t_data[1]
            to_find = "CIT{".encode()
            while data.find(to_find) != -1:
                first = data.find(to_find)
                last = data[first:].find("}".encode())
                if last == -1:
                    break
                if not self.isascii(data[first+4:first+last]):
                    break
                cits.append((t_data[0], str(str(data[first:first+last+1])[2:-1])))
                data = data[first + last + 1:]
        return list(set(cits))

    def run(self, data: bytes, *args) -> str:
        return self.find_CITS(data)