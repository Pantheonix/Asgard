# Changelog

## [1.0.1](https://github.com/Pantheonix/Asgard/compare/quetzalcoatl/v1.0.0...quetzalcoatl/v1.0.1) (2024-09-24)


### Bug Fixes

* **quetzalcoatl:** remove obsolete role enum ([#42](https://github.com/Pantheonix/Asgard/issues/42)) ([0794dfe](https://github.com/Pantheonix/Asgard/commit/0794dfe8d4f7210dc311a2685f27f22d60dda306))

## 1.0.0 (2024-09-21)


### Features

* **cors:** enable cors policy for anubis, enki and quetzalcoatl ([6652dbc](https://github.com/Pantheonix/Asgard/commit/6652dbcdbfaceeb94c36369423fecb7b2682d9d5))
* **images:** allow anonymous users to access the image endpoint ([9689d00](https://github.com/Pantheonix/Asgard/commit/9689d003b1b09b5c4b01abd0577a21844f26af8a))
* **roles:** add endpoints for roles management ([33e1b88](https://github.com/Pantheonix/Asgard/commit/33e1b88092bfd53c6e1aee14c6a05d36ec525f9e))
* **users:** add support for filtering-sorting-pagination for users endpoint ([464b181](https://github.com/Pantheonix/Asgard/commit/464b1810efe519e155309573d55da5603bcb0a53))


### Bug Fixes

* **quetzalcoatl:** check profile picture not to be null before adding its id to dtos ([f869580](https://github.com/Pantheonix/Asgard/commit/f8695804e16dc01123b4e70f6726d3555885bb58))
* **quetzalcoatl:** get total count of items after filtering for correct user pagination on GetAll endpoint ([8fdd7ec](https://github.com/Pantheonix/Asgard/commit/8fdd7ec9fbae2dc880b373abdab2f115bbd4e40d))
* **remove role:** add role information to user response ([c1ad42e](https://github.com/Pantheonix/Asgard/commit/c1ad42efd1e1f497e1a84009bea0f81aef9a2479))


### Performance Improvements

* **prod:** update SameSite attribute for cookies to None ([7d143ad](https://github.com/Pantheonix/Asgard/commit/7d143adde34b824adc3a90f6a062bb5453038912))
* **quetzalcoatl:** extract cors origins in envvar ([cffef24](https://github.com/Pantheonix/Asgard/commit/cffef24f5af89ee8c3328e0a8fd88e091c0f0939))
* **quetzalcoatl:** set cookies as samesite none ([6108f12](https://github.com/Pantheonix/Asgard/commit/6108f12a86e7060eab56506a7059bf13745bed5e))
* **quetzalcoatl:** simplify refresh token logic ([36f3105](https://github.com/Pantheonix/Asgard/commit/36f3105b35d0be6469a7afeae2a0f33a34ba0365))
* **quetzalcoatl:** update user dtos to use ProfilePictureId instead of ProfilePictureUrl ([67da983](https://github.com/Pantheonix/Asgard/commit/67da983e7155937f9ea956afa2717e7d9144837f))
* **register:** set auth tokens as samesite lax ([c4537b6](https://github.com/Pantheonix/Asgard/commit/c4537b6524e956215f278c32194b4a63a51634ba))
* **users:** define all fsp params as optional ([54ec5c2](https://github.com/Pantheonix/Asgard/commit/54ec5c24bebfe674ffaf973c1bb61b2b79cf5170))
