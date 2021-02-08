	select distinct ?coautor ?nombre from <http://graph.um.es/graph/sgi>
	where
	{		
		?doc <http://purl.org/roh/mirror/bibo#authorList> ?lista.
		?lista ?item ?autor.
		?lista ?itemCoautor ?coautor.
		?coautor a <http://purl.org/roh/mirror/foaf#Person>.
		?coautor <http://purl.org/roh/mirror/foaf#name> ?nombre.
		FILTER(?autor=<http://graph.um.es/res/person/1949f7bb-70d9-4e2b-94a4-a54b0df96312>)
		FILTER(?coautor!=?autor)
	}
