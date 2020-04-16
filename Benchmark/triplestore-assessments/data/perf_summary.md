# Summary of perforance analyses of RDF Triple Stores

This document provides a summary of the results of performance analyses of RDF triple store. It includes loading time, query response time and memory use from a number of references. We focused on analysis of performance using the LUBM benchmark to be consistent and since it is relevant to the considered scenario. We also include results from the Berlin SPARQL benchmark when relevant.

## Loading time:

### Summary
- Over 100KT/s, whatever the size, is good
- Allegro and Stardog are good
- Virtuoso OK
- Blazegraph OK
- Jena is bad
- Others are unknown

### Virtuoso:
- 30 to 37 KT/s for 1GT (virtuoso_lubm)

### Blazegraph:
- BT: 135 to 333 KT/s (w3c_large_stores)
- 10BT: 61 KT/s

### AllegroGraph:
- BT: 500KT/s (allegro_lubm) and 30 to 50 hours on GT (bench3)

### Jena:
- 10K: 9KT/s (jena_berlin_res)
- 100K: 16KT/s (jena_berlin_res)
- MT: 19KT/s (jena_berlin_res)
- 10MT: 20KT/s (jena_berlin_res) and 5 to 9 KT/s (jena_load_lubm)
- 100MT: 19KT/s (jena_berlin_res) 
- BT: 30 to 70 hours (5 to 9KT/s) (bench3) 

### Oracle:
- 30 to 60 hours on GT (bench3)

### Sesame:
- 30 to 60 hours on GT (bench3)

### Stardog
- 310 to 510 KT/s (stardog_benchmark_results)

### Neo4J
No result available

### Corese
No result available

## Query response time

### Summary
- Virtuoso and Allegro do good on large
- Stardog is good generally
- Blazegraph and stardog are OK on average
- Jena is pretty bad for large, OK for small
- Neo4J seems bad generally

### Virtuoso:
- 2 to 950ms on MT, 7 to 14K on 100MT (virtuoso_lubm)
- avg of 16.8 on 100MT (bench1)

### Blazegraph:
- Not great according to (bench5)
- Much better than Jena, close behind stardog, and well behind allegro and virtuoso according to (bench6)

### Allegro:
- BT: 7ms to 389K ms (allegro_lubm2) and Best ranking (bench3)

### Jena:
- 10KT: 6 to 37 ms (jena_berlin_res)
- 100KT: 7 to 357 ms (jena_berlin_res)
- MT: 10 to 3.6K ms (jena_berlin_res) and worst rank (bench3)
- 10MT: 32 to 22K ms (jena_berlin_res) and 40 to 230K ms only on Q1 and Q2 (jena_lubm_perf)
- 100MT: 41 to 84K ms (jena_lubm_perf) and avg of 35.1 (bench1)
- 500 to 1100ms on LUBM 1, 600 to 1443 on LUBM5, 691 to 9481 on LUMB10, 825 to 11796 on LUBM15 ()

### Sesame:
- avg of 164.7 on 100MT (bench1)
- second worst on GT (bench3)   

### Oracle:
- second best on GT (bench3)

### Stardog
- 10MT: 4 to 1.7K ms on LUBM100 (stardog_benchmark_results)
- 100MT: 6 to 23K on LUBM1000 and 2 to 170 ms on BSBM100M (stardog_benchmark_results)
- BT: 2 to 1.5K ms on BSBM1B (stardog_benchmark_results)
- 10BT: 6 to 11K ms on BSBM10B (stardog_benchmark_results)

### Neo4J
- In the order of seconds on LUBM10 to LUBM250 (neo4j_lubm)
- Indication that it is slow on Berlin Benchmark (neo4j_slow_query)

### Corese
No result available

## Memory usage

### Summary
- Allegro seems good
- Jena seems bad

### Jena:
- BT: last rank (bench3)

### Blazegraph
No result available

### Sesame:
- BT: third rank (bench3)

### Oracle:
- BT: second rank (bench3)

### AllegroGraph:
- BT: First rank (bench3)

### Stardog
No result available

### Neo4J 
No result available

### Corese
No result available

## References

@misc{virtuoso_lubm,
title={Virtuoso benchmarking LUBM},
url={http://vos.openlinksw.com/owiki/wiki/VOS/VOSArticleLUBMBenchmark}
}


@misc{allegro_lubm,
title={AllegroGraph overview including performance},
url={https://franz.com/agraph/allegrograph/}
}

@misc{allegro_lubm2,
title={AllegroGraph LUBM results},
url={https://franz.com/agraph/allegrograph/agraph_bench_lubm.lhtml}
}

@misc{jena_load_lubm,
title={Jena Loading Tests},
url={https://jena.apache.org/documentation/sdb/loading_performance.html}
}

@misc{jena_lubm_perf,
title={Jena LUBM perf},
url={https://jena.apache.org/documentation/sdb/query_performance.html}
}

@misc{bench1,
title={Benchmark 1},
url={file:///home/mdaquin/Downloads/2463676.2463718.pdf}
}

@misc{bench2,
title={Benchmark 2},
url={file:///home/mdaquin/Downloads/An_Efficient_Web_Ontology_Storage_Considering_Hierarchical_Knowledge_for_Jena-based_Applications.pdf}
}

@misc{bench3,
title={Benchmark 2},
url={http://www.ijaema.com/gallery/137-december-3030.pdf}
}

@misc{bench4,
title={Benchmark 4},
url={https://www.researchgate.net/profile/Ian_Emmons/publication/220830290_An_Evaluation_of_Triple-Store_Technologies_for_Large_Data_Stores/links/542015090cf2218008d43aa5/An-Evaluation-of-Triple-Store-Technologies-for-Large-Data-Stores.pdf}
}


@misc{jena_berlin_res,
title={Jena TDB result on Berlin SPARQL Benchmark},
url={https://lists.w3.org/Archives/Public/public-sparql-dev/2008JulSep/0029.html}
}

@misc{stardog_benchmark_results,
title={Stardog benchmark results},
url={https://docs.google.com/spreadsheets/d/1oHSWX_0ChZ61ofipZ1CMsW7OhyujioR28AfHzU9d56k/pubhtml#}
}

@misc{neo4j_slow_query,
title={Neo4J slow on query from berlin benchmark},
url={https://stackoverflow.com/questions/24346725/slow-berlin-sparql-benchmark-queries-in-neo4j}
}

@misc{neo4j_lubm,
title={Neo4J reasoning with LUBM},
url={https://neo4j.com/blog/neo4j-rdf-graph-database-reasoning-engine/}
}

@misc{w3c_large_stores,
title={Large RDF triple stores},
url={https://www.w3.org/wiki/LargeTripleStores}
}

@misc{bench5,
title={Benchmarking triple stores},
url={https://www.researchgate.net/publication/320864577_Benchmarking_RDF_Storage_Solutions_with_Iguana}
}

@misc{bench6,
title={Usability of triple stores},
url={https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4803025/}
}