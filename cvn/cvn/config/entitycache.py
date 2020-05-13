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
