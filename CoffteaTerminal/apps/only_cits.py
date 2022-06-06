class Cleaner():
    def __init__(self):
        self.name = "@:\\Cofftea\\CITonly\\>"

    def isCIT(self, data: str) -> bool:
        if "CIT{" in data and "}" in data:
            return True

    def run(self, data: list, *args) -> list:
        return [(flag[1], "") for flag in data if self.isCIT(flag[1])]