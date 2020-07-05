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


class Relationship:
    def __init__(
        self,
        ontology,
        name,
        inverse_ontology=None,
        inverse_name=None,
        link_to_cvn_person=False,
        parent=None,
    ):
        self.ontology = ontology
        self.name = name
        self.inverse_ontology = inverse_ontology
        self.inverse_name = inverse_name
        self.link_to_cvn_person = link_to_cvn_person
        self.parent = parent
