import re


def is_cvn_code_valid(code):
    valid = re.compile("\d\d\d.\d\d\d.\d\d\d.\d\d\d")
    return valid.match(code)
