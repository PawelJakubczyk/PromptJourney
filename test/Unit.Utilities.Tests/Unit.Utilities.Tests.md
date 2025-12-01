
---

# 🧪 Unit Test Project Overview

This document summarizes the structure and scope of the test suite, organized by namespace, class, and test group.

---

# 1. Namespace: `Unit.Utilities.Tests.Extensions`

## ▶ Class: `ErrorFactoryTests`

### Individual Tests

* `ChainedCalls_ShouldWorkCorrectly`
* `Create_ShouldReturnErrorWithDefaultMessage`
* `GetDetail_ShouldReturnCorrectDictionary_WithAllValues`
* `GetDetail_ShouldReturnDefaults_WhenValuesNotSet`
* `GetErrorCode_ShouldReturnErrorCode_WhenExists`
* `GetErrorCode_ShouldReturnNull_WhenNotExists`
* `GetErrorCode_ShouldReturnNull_WhenValueIsNotInt`
* `GetLayer_ShouldReturnLayer_WhenExists`
* `GetLayer_ShouldReturnNull_WhenNotExists`
* `GetLayer_ShouldReturnNull_WhenValueIsNotString`
* `WithErrorCode_ShouldAddErrorCodeToMetadata`
* `WithErrorCode_WithNullValue_ShouldNotAddErrorCodeToMetadata`
* `WithLayer_ShouldAddLayerToMetadata_AndReturnSameBuilder`
* `WithMessage_ShouldCreateErrorWithNewMessage`
* `WithMessage_ShouldPreserveMetadata`

### Test Group: `WithErrorCode_ShouldHandleCommonStatusCodes`

* `statusCode: 200`
* `statusCode: 400`
* `statusCode: 401`
* `statusCode: 404`
* `statusCode: 500`

### Test Group: `WithLayer_ShouldHandleAllLayerTypes`

* `ApplicationLayer`
* `DomainLayer`
* `InfrastructureLayer`
* `PersistenceLayer`
* `PresentationLayer`
* `UtilitiesLayer`

---

# 2. Namespace: `Unit.Utilities.Tests.Workflow`

## ▶ Class: `WorkflowPipelineTests`

### Test Group: `Create_ShouldRespectBreakOnErrorParameter`

* `breakOnError: False`
* `breakOnError: True`

### Individual Tests

* `Create_WithDefaultBreakOnError_ShouldDefaultToTrue`
* `Create_WithEmptyErrors_ShouldReturnPipelineWithEmptyErrors`
* `Create_WithErrors_ShouldReturnPipelineWithErrors`
* `Create_WithNullErrors_ShouldReturnPipelineWithEmptyErrors`
* `Empty_ShouldReturnPipelineWithNoErrors`
* `EmptyAsync_ShouldReturnTaskWithEmptyPipeline`
* `GetResult_ShouldReturnCorrectValue`
* `GetResult_WhenKeyNotExists_ShouldReturnDefault`
* `GetResult_WithKey_ShouldReturnCorrectValue`
* `Multiple_Create_Calls_ShouldReturnIndependentInstances`
* `SetResult_ShouldOverwriteExistingValue`
* `WorkflowPipeline_ShouldHandleLargeNumberOfErrors`
* `WorkflowPipeline_ShouldMaintainErrorOrder`

---

Jeśli chcesz, mogę:

✔ Przerobić to na **README.md**
✔ Dodać sekcję o pokryciu testów, konwencjach lub architekturze
✔ Wygenerować wersję bardziej opisową (np. co test jest odpowiedzialny za co)
