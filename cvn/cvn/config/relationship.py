class Relationship:
    def __init__(self, ontology, name, inverse_ontology=None, inverse_name=None, link_to_cvn_person=False):
        self.ontology = ontology
        self.name = name
        self.inverse_ontology = inverse_ontology
        self.inverse_name = inverse_name
        self.link_to_cvn_person = link_to_cvn_person
