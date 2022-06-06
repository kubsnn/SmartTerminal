import os
import shutil
class Pypy:
    def __init__(self):
        self.name = "@:\\Cofftea\\Pypy\\>"
    
    def cmd_exists(self, cmd):
        print(shutil.which("pypy3"))
        return shutil.which(cmd) is not None

    def run(self, data, modules):
        if self.cmd_exists("pypy3"):
            path = os.path.dirname(os.path.realpath(__file__)) + "\\binarywalk.py "
            print(path)
            os.system("pypy3 " + path + " ".join(modules[1:]))
            modules = []
            return ["Program above calculated in pypy3", ""]
        else:
            return data