# Contributing guidelines
Glad to see you here. There are many different ways in which you can contribute. One of them is simply by using KInspector and providing us with the feedback. Or you can code some new features. We welcome all the contribution activity. Before you start, please read this short guide to save us some time and trouble.


## I have an idea for a new feature
Everybody loves new features! You can create a new issue or you can code it on your own and provide us with the pull request. Anyway don't forget to mention what's the use case and what's the expected output.


## I found a bug
Sorry to hear that. Create a new issue and label it as a bug. Before you send the issue, make sure that you include all the important information like

- Detailed description of the issue
- What was the version
- Screenshot (if possible)
- Error message
- What is an expected behavior

The more information you provide, the better. You can also fix the bug and submit a new pull request.


## Submitting pull request
We use [feature branch workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/feature-branch-workflow). Fork this repository, create a new branch and start coding.

Before you send us new pull request, make sure that you meet following requirements

- Code is buildable
- All tests are green
- Module runs on every instance defined in module metadata
- Code follows the .NET [Framework Design Guidelines](https://msdn.microsoft.com/en-us/library/ms229042.aspx)
    - If you're not sure about some rule, follow the style of the existing code.


### Example - process of contribution
1. ```Tom``` forks this repository
2. Creates new branch for his ```cool``` feature
3. Hacks his code
4. Tests it!
5. Once he's happy with the module, he submits a pull request from his ```cool``` branch to this ```master``` branch
6. We discuss the pull request with ```Tom``` and maybe suggest some adjustments
7. Once the module is ready, we will merge it


### Testing
Unfortunately, we don't have an environment for integration tests ready yet (we're [working on it](https://github.com/Kentico/KInspector/issues/13)). That means you must test your module manually. Make sure that your module runs on every supported Kentico CMS instances that you declared in the module metadata. Meanwhile, you can write some unit tests.


### Learn how to write good commit messages
We hate sloppy commit messages. No more ```Performance tuning``` or ```Changed a few files```. Please read the following articles

- [Writing good commit messages](https://github.com/erlang/otp/wiki/Writing-good-commit-messages)
- [A Note About Git Commit Messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)
- [On commit messages](http://who-t.blogspot.com/2009/12/on-commit-messages.html)


## Questions & Answers
Use [Stack Overflow](https://stackoverflow.com/) to ask questions. Make sure that your question contains [Kentico](https://stackoverflow.com/questions/tagged/kentico) tag, otherwise we won't be able to find it. Please don't use GitHub Issues for questions. They're meant for tracking features and bugs.
