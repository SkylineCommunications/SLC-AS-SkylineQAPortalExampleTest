# QAPortal example scripts

This solution contains example automation scripts that can be used to get started with the [Skyline QAPortal](https://catalog.dataminer.services/details/63e23e06-1360-4e1e-920a-0cf25a9ab346). Below you can find an overview of all the scripts that can be found in this repository.

## <ins>Regression Test Example</ins>

### ExampleTest

This example script that can be used to add a regression test to the **Regression Tests** table under the **General** page.

Folder: Regression Testing

Cases:
| Case Name | Description |
| --------- | ----------- |
| Get Root View | Succeeds if a view is returned with the ID '-1'.  |

Performance cases:
| Case Name | Description | Performance Value |
| --------- | ----------- | ----------------- |
| Retrieving elements | Succeeds if the elements are returned. | The time it took to retrieve the elements. |
| Retrieving services | Succeeds if the services are returned. | The time it took to retrieve the services. |

## <ins>Scalability Test Example</ins>

The scripts in this sections can be used to build a scalability test. In this example we are going to create a test view and add more views to see the performance impact on having more views in the system.

### ExampleScaleInitTest

This script initializes the scalability test example. This script will run at the start of the scalability test. In this example, it will create a view 'Scalability Test View' in the root view.

Folder: Regression Testing

### ExampleScaleFinalizeTest

This script finalizes the scalability test example. This script will run at the end of the scalability test. In this example, the view 'Scalability Test View' will be deleted with all its child views.

Folder: Regression Testing

### ExampleScaleUpTest

This script will be used to scale up the system with a given step size. In this example, child views will be created under the view 'Scalability Test View'.

Folder: Regression Testing

### ExampleScalePerfTest

This script will be used to measure the performance after every scale up step.

Folder: Regression Testing

Cases:
| Case Name | Description |
| --------- | ----------- |

Performance cases:
| Case Name | Description | Performance Value |
| --------- | ----------- | ----------------- |
| Rename view | Rename one view | How long it took to rename the view |
| Retrieving views | Retrieve all views in the system | How long it took to retrieve all views in the system |

