import os
class Installer:
    def __init__(self):
        self.name = "@:\\Cofftea\\Installer\\>"
    
    def driver(self, data):
        try:
            import magic
            return [("Nothing to install", "")]
        except:
            data = []
            os.system("pip install python-magic-bin")
            data.append(("Installed magic", ""))
            return data
        
    def run(self, data, *args):
        return self.driver(data)