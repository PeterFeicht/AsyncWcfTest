AsyncWcfTest
============

Test project to reproduce a bug with async WCF calls.

After asking [this question](http://stackoverflow.com/questions/32475781/)
on StackOverflow I tried to reproduce the problem, this project is the result.

See the question for some more details.

Usage
-----

To use the test application just compile, run `AsyncWcfServer`, and then start
`AsyncWcfClient`.

You can change some settings using the GUI, but you'll have to look at the code
to make sense of them. Those are just there so you can test different
configurations, the default settings will cause the deadlock.
