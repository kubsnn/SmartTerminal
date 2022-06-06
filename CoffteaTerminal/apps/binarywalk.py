from distutils.command.clean import clean
import glob
import os
import sys
try:
    from colorama import Fore    
    import colorama
except:
    os.system("pip install colorama")
    from colorama import Fore    
    import colorama
import codecs
import base64
import importlib.util
import inspect
from math import log10 as l10

class Loader():
    def __init__(self, path):
        self.path = path

    def load(self):
        spec = importlib.util.spec_from_file_location("new_class", self.path+".py")
        func = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(func)
        return inspect.getmembers(func, inspect.isclass)[0][1]()

class Binwalk():
    def __init__(self, modules):
        self.modules = modules
        self.path = "."
        self.files = glob.glob(self.path + "/**/*", recursive=True)
        self.files.sort()
        self.decoder = None
        self.data = []

    def print_results(self):
        print(Fore.RED + self.decoder.name)
        dic_names = {i[0]:[] for i in self.data}
        for i in self.data:
            dic_names[i[0]].append(i[1])
        dic_names = {
            k: [
                (str(i+1), dic_names[k][i])
                for i in range(len(dic_names[k]))
            ]
            for k in dic_names.keys()
        }
        max_right = int(
            l10(max_right) 
            if (max_right := max(
                inner if len(inner:=[max(
                [i for i in range(len(dic_names[k]))]
                ) 
            for k in dic_names.keys()]) > 0 else [0])) > 1 
            else 0
        )
        for i in dic_names.keys():
            print(2 * " " + Fore.CYAN + i)
            if len("".join([x[1] for x in dic_names[i]])) != 0:
                for dx, o in enumerate(dic_names[i]):
                    print(
                        4 * " " + 
                        (max_right-int(l10(int(o[0])))) * " " + 
                        Fore.LIGHTRED_EX + 
                        o[0] + 
                        " " +
                        (Fore.LIGHTWHITE_EX if dx % 2 == 0 else Fore.WHITE) +
                        o[1]
                    )
                print()

    def read_file(self):
        for file in self.files:
            #check if file is a directory
            if os.path.isdir(file):
                continue
            if file.split(".")[-1] != "py" and file.split(".")[-1] != "pyc":
                with open(file, "rb") as f:
                    self.data.append((file, f.read()+b"\r\n"))

    def run(self):
        self.read_file()

        #print(self.modules)
        for module in self.modules:
            self.decoder = Loader(os.path.dirname(os.path.realpath(__file__)) + "\\" + module).load()
            self.data = self.decoder.run(self.data, self.modules)
            if len(self.data) > 0 and type(self.data[0]) == str:
                self.data = [(self.data[0], "")]
                break

        self.print_results()


if __name__ == "__main__":
    colorama.init()
    bw = Binwalk(sys.argv[1:])
    bw.run()