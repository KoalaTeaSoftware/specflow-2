# Specflow Core Test Framework
This codebase provides a very basic specflow test mechanism for providing smoke and acceptance tests of a web site (coded in a separate repository).

The user should be able to easily clone this, alter some fundamental fixtures' values, then extend it to provide tests for any system under test (SUT).

It provides the following capabilities:
* webdriver management - the user does not have to consider this when extending the suite 
* A html report of every run, listing passed test and providing information about failures, each with a screen-grab to aid in debugging.
* Core capabilities that have integrated wait and context possibilities (that default to values provided as test configuration) to
  * Find elements
  * Find links by their user-viewable text
  * Check that links provide a success HTTP response (faster than clicking and waiting)
  * Check that clicking on links takes you to a correct page
  * Simplify browser tab management
