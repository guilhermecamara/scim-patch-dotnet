# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="2.0.1"></a>
## 2.0.1 (2024-08-22)

### Bug Fixes

* **operationnode:** added more information to argumentexception when path is not found

<a name="2.0.0"></a>
## 2.0.0 (2024-08-20)

### Bug Fixes

* fixed remove operation, refactor opration to operationnode code and fixed tests
* fixed remove operations not working

### Breaking Changes

* fixed remove operations not working

<a name="1.0.0"></a>
## 1.0.0 (2024-08-19)

### Features

* added interpetrer for scimfilter.g4 (WIP)
* added strategy for applying add operations
* create an add OperationNode from an Operation
* initial implementation of an object path resolver
* initial implementation of operations deserializer using newtonsoft
* **defaultaddoperationstrategy:** implementation of add operation on IList? and IList targets
* **pathresolver:** added filter apply on enumerables

