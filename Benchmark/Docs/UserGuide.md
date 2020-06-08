# Hercules ASIO Benchmark - User guide

This document gives a brief overview of the usage of the interface to the triple store benchmark data developped as part of the project. The primary function of this interface is to display a ranking of triple store systems according to a score based on assessing them with the set of weighted criteria considered. However, this interface being based on linked data, it also enables exploring the criteria and their assessement for different triple store systems, as well as to provide personalised weights for the criteria that can better reflect specific scenarios and use cases.

## Triple store ranking

Starting on the [frontpage of the tool](http://herc-as-front-desa.atica.um.es/benchmark), the ranking of triple stores can be reached be clicking on the number of known triple stores (28 at the time of writing), or on "Stores" in the navigation bar. This will show the list of triple stores that have been assessed in descending order of their score, and the score. It also shows a list of known triple stores that have not yet been assessed.

## Exploring assessements for a triple store system

Clicking on the row corresponding to a ranked striple store system in the previous page will show the scores obtained by this striple store in each of the categories of assessement. Any of those categories can be further expanded by clicking on it, showing the list of criteria assessed for the given triple store, in the given category, together with the score received, and explaination for the score and a link to evidence to justify that score.

## Exploring assessement criteria

More information about the criteria used for assessment can be obtained either by clicking on an individual criteria in the previous page, or by clicking on either "Benchmark" or the number of assessed criteria (currently 57) in the frontpage of the tool.

For each criteria, a short description of the criteria and the way it is assessed is provided, as well as a list of the scores obtained by each assessed triple store is provided (linking back to the page for the triple store).

## Personalising the weights of criteria

In the previous page for a given criteria, the default weight associated with the criteria is displayed and can be modified to check how changing this weight can affect the ranking of triple store systems. This affects one criteron at a time and gets "reset" when leaving the page.

It is possible to create a fully personalised ranking by changing the weights of all criteria and categories. In the ["benchmark" page](http://herc-as-front-desa.atica.um.es/benchmark/criterion), checking the "Adjust weights" button will enable editing weights. Once that is done, the weights appear next to each of the criteria and categories and can be edited directly on the page, by clicking on the weight to be modifed, and using the appearing slider. Weights customised in this way are stored locally and will continue being valid for the user until the "Adjust weights" button is unchecked. The ranking of triple store is automatically affected by editing the weights. Note that changes in the weights are not transferred from one browser to the other. They will remain active as long as the tool with the same browser on the same computer.

## Adding and Editing Assessments

The interface currently does not provide a function to change the underlying data, by adding assessments, criterion or default weights, or by changing the ones in place. This can currently be achieved by modifying the github projects associated with producing the data for those, e.g. through a pull request. More details about this will be provided in the developper documentation and an adminstrator interface providing those functionalities will be developped in the near future.

