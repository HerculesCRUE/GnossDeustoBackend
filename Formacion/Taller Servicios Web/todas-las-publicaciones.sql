select ?doc ?titulo from <http://graph.um.es/graph/sgi>
	where
	{
		?doc <http://purl.org/roh#title> ?titulo.
		?doc <http://purl.org/roh/mirror/bibo#authorList> ?lista.
		?lista ?item <http://graph.um.es/res/person/1949f7bb-70d9-4e2b-94a4-a54b0df96312>
	}
	
