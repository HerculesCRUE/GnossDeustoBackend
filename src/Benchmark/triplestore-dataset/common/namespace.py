from rdflib import Namespace

__all__ = [
    'ASIO']

_prefix_main = 'http://datascienceinstitute.ie/'

ASIO = Namespace(''.join([ _prefix_main, 'asio', '/']))