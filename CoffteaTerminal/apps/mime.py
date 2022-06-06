import magic
class Magic:
    def __init__(self) -> None:
        self.name = "@:\\Cofftea\\Magic\\>"

    def driver(self, data):
        return [(i[0], magic.from_buffer(i[1], mime=True)) for i in data]
    
    def run(self, data: bytes, *args) -> str:
        return self.driver(data)