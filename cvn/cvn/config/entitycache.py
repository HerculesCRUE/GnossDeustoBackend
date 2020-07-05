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


class EntityCache:
    def __init__(self):
        self.cached = {}

    def add_to_cache(self, key, value):
        self.cached[key] = value

    def in_cache(self, key):
        return key in self.cached

    def get(self, key):
        try:
            return self.cached[key]
        except KeyError:
            return None


__cache = EntityCache()


def get_current_entity_cache():
    return __cache
