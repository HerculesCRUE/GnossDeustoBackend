#  This file is part of Hércules ASIO.
#
#  Hércules ASIO is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#
#  Hércules ASIO is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#
#  You should have received a copy of the GNU General Public License
#  along with Hércules ASIO.  If not, see <https://www.gnu.org/licenses/>.
#

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


if __name__ == "__main__":
    unittest.main()
