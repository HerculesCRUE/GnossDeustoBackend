import unittest
from cvn.config.entity import Entity


class TestConstructor(unittest.TestCase):
    def test_constructor(self):
        entity = Entity("060.010.010.000", "bibo", "AcademicArticle", None, None, None)
        self.assertEqual(entity.code, "060.010.010.000")
        self.assertEqual(entity.ontology, "bibo")
        self.assertEqual(entity.classname, "AcademicArticle")
        self.assertIsNone(entity.parent)
        self.assertIsNone(entity.identifier_config_resource)
        self.assertIsNone(entity.identifier_config_format)


if __name__ == '__main__':
    unittest.main()
