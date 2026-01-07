# ✅ **ExampleLinksController – `api/examplelinks`**

---

## Commands

---

### **POST `/api/examplelinks`**

**Purpose:** Adds a new example link to the system.
**Results:**

* 201 Created — returns location of new link
* 409 Conflict — duplicate
* 404 NotFound — referenced entity missing
* 400 BadRequest

```
Create_ShouldReturnCreated_WhenLinkIsAdded
Create_ShouldReturnConflict_WhenLinkAlreadyExists
Create_ShouldReturnNotFound_WhenVersionDoesNotExist
Create_ShouldReturnNotFound_WhenStyleDoesNotExist
Create_ShouldReturnNotFound_WhenVersionAndStyleDoesNotExist
Create_ShouldReturnBadRequest_WhenExampleLinkIsInvalid (Invalid URL/TooLong/Whitespace/null)
Create_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (200 / Some more)
Create_ShouldHandleCancellationToken
```

---

### **DELETE `/api/examplelinks/{link}`**

**Purpose:** Deletes a specific example link by ID.
**Results:**

* 200 OK — deletion details
* 404 NotFound
* 400 BadRequest

```
Delete_ShouldReturnOk_WhenLinkIsDeleted
Delete_ShouldReturnNotFound_WhenLinkDoesNotExist
Delete_ShouldReturnBadRequest_WhenInvalidLink (TooLong/Whitespace/Null/Invalid Guid)
Delete_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / Some more)
Delete_ShouldHandleCancellationToken
```

---

### **DELETE `/api/examplelinks/style/{styleName}`**

**Purpose:** Deletes all example links for a specific style.
**Results:**

* 200 OK — bulk deletion info
* 404 NotFound
* 400 BadRequest

```
DeleteByStyle_ShouldReturnOk_WhenLinksAreDeleted
DeleteByStyle_ShouldReturnNotFound_WhenStyleDoesNotExist
DeleteByStyle_ShouldReturnBadRequest_WhenStyleNameInvalid (TooLong/Whitespace/null)
DeleteByStyle_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / Some more)
DeleteByStyle_ShouldHandleCancellationToken
```

---

## Queries

---

### **GET `/api/examplelinks/no-empty`**

**Purpose:** Checks whether at least one example link exists (global existence check).
**Results:**

* 200 OK — boolean

```
CheckAnyExists_ShouldReturnTrue_WhenAnyLinkExists
CheckAnyExists_ShouldReturnFalse_WhenNoLinksExist
CheckAnyExists_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / Some more)
CheckAnyExists_ShouldHandleCancellationToken
CheckAnyExists_ShouldUseSingletonQuery
```

---

### **GET `/api/examplelinks/link/{id}/exists`**

**Purpose:** Checks whether any example link exists for the specified link (URL).
**Results:**

* 200 OK — boolean
* 400 BadRequest

```
CheckExistsByLink_ShouldReturnTrue_WhenLinkExists 
CheckExistsByLink_ShouldReturnFalse_WhenLinkDoesNotExist 
CheckExistsByLink_ShouldReturnBadRequest_WhenLinkIsInvalid (Invalid URL/TooLong/Whitespace/null) 
CheckExistsByLink_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / Some more) 
CheckExistsByLink_ShouldHandleCancellationToken
```

---

### **GET `/api/examplelinks/{link}/exists`**

**Purpose:** Checks whether a given link ID exists in the system.
**Results:**

* 200 OK — boolean result
* 400 BadRequest

```
CheckExists_ShouldReturnTrue_WhenLinkExists
CheckExists_ShouldReturnFalse_WhenLinkDoesNotExist
CheckExists_ShouldReturnBadRequest_WhenInvalidLink (TooLong/Whitespace/Null/Invalid Guid)
CheckExists_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / Some more)
CheckExists_ShouldHandleCancellationToken
```

---

### **GET `/api/examplelinks/style/{styleName}/exists`**

**Purpose:** Checks whether any links exist for a specific style.
**Results:**

* 200 OK — boolean
* 404 NotFound — style not found
* 400 BadRequest

```
CheckStyleHasAnyLinks_ShouldReturnTrue_WhenLinksExistForStyle
CheckStyleHasAnyLinks_ShouldReturnFalse_WhenNoLinksExistForStyle
CheckStyleHasAnyLinks_ShouldReturnNotFound_WhenStyleDoesNotExist
CheckStyleHasAnyLinks_ShouldReturnBadRequest_WhenInvalidStyleName (TooLong/Whitespace/null)
CheckStyleHasAnyLinks_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / Some more)
CheckStyleHasAnyLinks_ShouldHandleCancellationToken
```

---

### **GET `/api/examplelinks`**

**Purpose:** Returns all example links stored in the system.
**Results:**

* 200 OK — list of example links

```
GetAll_ShouldReturnEmptyList_WhenThereIsNoData
GetAll_ShouldReturnListOfExampleLinks_WhenDataExists
GetAll_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / Some more)
GetAll_ShouldHandleCancellationToken
GetAll_ShouldUseSingletonQuery
```

---

### **GET `/api/examplelinks/{id}`**

**Purpose:** Returns a single example link by its ID.
**Results:**

* 200 OK — example link
* 404 NotFound — link not found
* 400 BadRequest

```
GetById_ShouldReturnExampleLink_WhenLinkExists
GetById_ShouldReturnNotFound_WhenLinkDoesNotExist
GetById_ShouldReturnBadRequest_WhenInvalidId (TooLong/Whitespace/Null/Invalid Guid)
GetById_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / Some more)
GetById_ShouldHandleCancellationToken
```

---

### **GET `/api/examplelinks/link/{link}`**

**Purpose:** Returns all example links matching the specified link (URL).
**Results:**

* 200 OK — list (can be empty)
* 400 BadRequest

```
GetByLink_ShouldReturnList_WhenLinkExists
GetByLink_ShouldReturnEmptyList_WhenNoMatches
GetByLink_ShouldReturnBadRequest_WhenLinkIsInvalid (Invalid URL/TooLong/Whitespace/null)
GetByLink_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / Some more)
GetByLink_ShouldHandleCancellationToken
```

---

### **GET `/api/examplelinks/style/{styleName}`**

**Purpose:** Returns all example links associated with a specific style.
**Results:**

* 200 OK — links for the style
* 404 NotFound — style not found
* 400 BadRequest

```
GetByStyle_ShouldReturnList_WhenStyleExistsAndHaveExampleLinks
GetByStyle_ShouldReturnEmptyList_WhenStyleHasNoLinks
GetByStyle_ShouldReturnNotFound_WhenStyleDoesNotExist
GetByStyle_ShouldReturnBadRequest_WhenStyleNameIsInvalid (TooLong/Whitespace/null)
GetByStyle_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / Some more)
GetByStyle_ShouldHandleCancellationToken
```

---

### **GET `/api/examplelinks/style/{styleName}/version/{version}`**

**Purpose:** Returns example links for a given style and version combination.
**Results:**

* 200 OK
* 404 NotFound — no links for given style & version or style/version do not exist
* 400 BadRequest

```
GetByStyleAndVersion_ShouldReturnList_WhenVersionAndStyleExistAndExampleLinksExists
GetByStyleAndVersion_ShouldReturnEmptyList_WhenStyleAndVersionExistButCombinationHaveNoLinks
GetByStyleAndVersion_ShouldReturnNotFound_WhenStyleDoesNotExist
GetByStyleAndVersion_ShouldReturnNotFound_WhenVersionDoesNotExist
GetByStyleAndVersion_ShouldReturnNotFound_WhenStyleAndVersionDoesNotExist
GetByStyleAndVersion_ShouldReturnBadRequest_WhenVersionInvalid (TooLong/Whitespoace/Wrong Format/null)
GetByStyleAndVersion_ShouldReturnBadRequest_WhenStyleInvalid (TooLong/Whitespace/null)
GetByStyleAndVersion_ShouldReturnBadRequest_WhenVersionAndStyleInvalid (TooLong/Whitespoace/Wrong Format/null) (TooLong/Whitespace/null)
GetByStyleAndVersion__ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / Some more)
GetByStyleAndVersion_ShouldHandleCancellationToken
```

---


# 🟦 **PromptHistoriesController – `api/prompthistories`**

---

## Commands

---

### **POST `/api/prompthistories`**

**Purpose:** Adds a new prompt entry to the history.
**Results:**

* 201 Created
* 404 NotFound
* 409 Conflict
* 400 BadRequest

```
Create_ShouldReturnCreated_WhenPromptIsAdded
Create_ShouldReturnConflict_WhenDuplicatePromptDetected
Create_ShouldReturnBadRequest_WhenPromptIsInvalid (TooLong/Whitespace/Null)
Create_ShouldReturnBadRequest_WhenVersionIsInvalid (TooLong/WrongFormat/Null/Whitespace)
Create_ShouldReturnCreated_WhenVersionAndPromptAreInvalid (TooLong/WrongFormat/Null/Whitespace)
Create_ShouldReturnNotFound_WhenVersionNotExists
Create_ShouldReturnNotFound_WhenStyleNotExists
Create_ShouldReturnNotFound_WhenVersionAndStyleNotExists
Create_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (200 / Some more)
Create_ShouldHandleCancellationToken
```

---

### **DELETE `/api/prompthistories/{id}`**

**Purpose:** Deletes a prompt history record by ID.
**Results:**

* 200 OK — deletion details
* 404 NotFound
* 400 BadRequest

```
Delete_ShouldReturnOk_WhenHistoryRecordIsDeleted 
Delete_ShouldReturnNotFound_WhenHistoryRecordDoesNotExist 
Delete_ShouldReturnBadRequest_WhenIdIsInvalid (Whitespace/Null/Invalid Guid) 
Delete_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / Some more) 
Delete_ShouldHandleCancellationToken
```

---

## Queries

---

### **GET `/api/prompthistories/count`**

**Purpose:** Returns the total number of stored prompt records.
**Results:** 

* 200 OK

```
GetRecordCount_ShouldReturnZero_WhenNoRecordsExist
GetRecordCount_ShouldReturnTotalCount_WhenRecordsExist
GetRecordCount_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / Some more)
GetRecordCount_ShouldHandleCancellationToken
GetRecordCount_ShouldUseSingletonQuery
```

---

### **GET `/api/prompthistories`**

**Purpose:** Returns all saved prompt history records.
**Results:** 

* 200 OK

```
GetAll_ShouldReturnEmptyList_WhenThereIsNoData
GetAll_ShouldReturnListOfPromptHistories_WhenDataExists
GetAll_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / Some more)
GetAll_ShouldHandleCancellationToken
GetAll_ShouldUseSingletonQuery
```

---

### **GET `/api/prompthistories/date-range?from=...&to=...`**

**Purpose:** Returns prompts created within a specific date range.
**Results:** 

* 200 OK
* 400 BadRequest

```
GetByDateRange_ShouldReturnRecordsWithinRange_WhenRecordsExist
GetByDateRange_ShouldReturnRecordsWithinRange_WhenRangeExceedsExistedRecords
GetByDateRange_ShouldReturnEmptyList_WhenNoRecordsInRange
GetByDateRange_ShouldReturnBadRequest_WhenFromIsInvalid (AfterTo / InvalidFormat / Null / Whitespace)
GetByDateRange_ShouldReturnBadRequest_WhenToIsInvalid (AfterFrom / InvalidFormat / Null / Whitespace)
GetByDateRange_ShouldReturnBadRequest_WhenFromAndToIsInvalid (InvalidFormat / Null / Whitespace)
GetByDateRange_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / Some more)
GetByDateRange_ShouldHandleCancellationToken
```

---

### **GET `/api/prompthistories/keyword/{keyword}`**

**Purpose:** Returns all prompts containing a given keyword.
**Results:** 

* 200 OK
* 400 BadRequest

```
GetByKeyword_ShouldReturnRecordsContainingKeyword_WhenMatchesExist
GetByKeyword_ShouldReturnEmptyList_WhenNoMatchesFound
GetByKeyword_ShouldReturnBadRequest_WhenKeywordIsInvalid (Too Long / Whitespace / Null)
GetByKeyword_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / Some more)
GetByKeyword_ShouldHandleCancellationToken
```

---

### **GET `/api/prompthistories/last/{count}`**

**Purpose:** Returns the last X prompt history entries.
**Results:** 

* 200 OK
* 400 BadRequest

```
GetLast_ShouldReturnLastNRecords_WhenThereAreAtLeastNRecords
GetLast_ShouldReturnAllAvailableRecords_WhenThereAreFewerThanN
GetLast_ShouldReturnEmptyList_WhenNoRecordsExist
GetLast_ShouldReturnBadRequest_WhenCountIsInvalid (Zero / Negative / TooLarge / InvalidFormat)
GetLast_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / Some more)
GetLast_ShouldHandleCancellationToken
```

---


# 🟩 **PropertiesController – `api/properties`**

---

## Commands

---

### **POST `/api/properties`**

**Purpose:** Adds a new property definition.
**Results:**

* 201 Created
* 409 Conflict
* 404 NotFound
* 400 BadRequest

```
CreateProperty_ShouldReturnCreated_WhenPropertyIsAdded
CreateProperty_ShouldReturnConflict_WhenPropertyAlreadyExists
CreateProperty_ShouldReturnNotFound_WhenVersionDoesNotExist
CreateProperty_ShouldReturnBadRequest_WhenRequestIsInvalid (VersionInvalid/PropertyNameInvalid/ParametersInvalid/DefaultValueInvalid/MinMaxInvalid/DescriptionInvalid With Difrent Configurations)
CreateProperty_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (200 / 409 / SomeMore)
CreateProperty_ShouldHandleCancellationToken
```

---

### **DELETE `/api/properties/version/{version}/{propertyName}`**

**Purpose:** Deletes a property from a specific version.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
DeleteProperty_ShouldReturnOk_WhenPropertyDeleted
DeleteProperty_ShouldReturnNotFound_WhenPropertyDoesNotExist
DeleteProperty_ShouldReturnNotFound_WhenVersionDoesNotExist
DeleteProperty_ShouldReturnNotFound_WhenPropertyAndVersionDoesNotExist
DeleteProperty_ShouldReturnBadRequest_WhenVersionIsInvalid (TooLong/Whitespace/Null/WrongFormat)
DeleteProperty_ShouldReturnBadRequest_WhenPropertyNameIsInvalid (TooLong/Whitespace/Null/WrongFormat)
DeleteProperty_ShouldReturnBadRequest_WhenVersionAndPropertyNameIsInvalid (TooLong/Whitespace/Null/WrongFormat)
DeleteProperty_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
DeleteProperty_ShouldHandleCancellationToken
```

---

### **PATCH `/api/properties`**

**Purpose:** Partially updates a property (only a chosen attribute).
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
PatchProperty_ShouldReturnOk_WhenPropertyCharacteristicUpdated
PatchProperty_ShouldReturnNotFound_WhenPropertyDoesNotExist
PatchProperty_ShouldReturnNotFound_WhenVersionDoesNotExist
PatchProperty_ShouldReturnNotFound_WhenPropertyAndVersionDoesNotExist
PatchProperty_ShouldReturnBadRequest_WhenRequestIsInvalid (VersionInvalid/PropertyNameInvalid/CharacteristicInvalid/NewValueInvalid)
PatchProperty_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore)
PatchProperty_ShouldHandleCancellationToken
```

---

### **PUT `/api/properties`**

**Purpose:** Updates an existing property (full update).
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
UpdateProperty_ShouldReturnOk_WhenPropertyUpdated
UpdateProperty_ShouldReturnNotFound_WhenPropertyDoesNotExist
UpdateProperty_ShouldReturnNotFound_WhenVersionDoesNotExist
UpdateProperty_ShouldReturnNotFound_WhenPropertyAndVersionDoesNotExist
UpdateProperty_ShouldReturnBadRequest_WhenRequestIsInvalid (VersionInvalid/PropertyNameInvalid/ParametersInvalid/DefaultValueInvalid/MinMaxInvalid/DescriptionInvalid)
UpdateProperty_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
UpdateProperty_ShouldHandleCancellationToken
```

---

## Queries

---

### **GET `/api/properties/{version}/{propertyName}/exists`**

**Purpose:** Checks whether a specific property exists in a given version.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
CheckPropertyExists_ShouldReturnTrue_WhenPropertyExistsInVersion
CheckPropertyExists_ShouldReturnFalse_WhenPropertyDoesNotExistInVersion
CheckPropertyExists_ShouldReturnBadRequest_WhenVersionIsInvalid (TooLong/Whitespace/Null/WrongFormat)
CheckPropertyExists_ShouldReturnBadRequest_WhenPropertyNameIsInvalid (TooLong/Whitespace/Null/WrongFormat)
CheckPropertyExists_ShouldReturnBadRequest_WhenVersionOrPropertyNameIsInvalid (TooLong/Whitespace/Null/WrongFormat)
CheckPropertyExists_ShouldReturnNotFound_WhenVersionDoesNotExist
CheckPropertyExists_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore)
CheckPropertyExists_ShouldHandleCancellationToken
```

---

### **GET `/api/properties`**

**Purpose:** Returns all properties across all versions.
**Results:** 

* 200 OK

```
GetAll_ShouldReturnEmptyList_WhenThereIsNoData
GetAll_ShouldReturnListOfProperties_WhenDataExists
GetAll_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 400 / 404 / 409 / SomeMore)
GetAll_ShouldHandleCancellationToken
GetAll_ShouldUseSingletonQuery
```

---

### **GET `/api/properties/{version}`**

**Purpose:** Gets all properties available for a specific model version.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
GetAllPropertiesByVersion_ShouldReturnList_WhenVersionExists
GetAllPropertiesByVersion_ShouldReturnEmptyList_WhenVersionExistsButNoProperties
GetAllPropertiesByVersion_ShouldReturnNotFound_WhenVersionDoesNotExist
GetAllPropertiesByVersion_ShouldReturnBadRequest_WhenVersionIsInvalid (TooLong/Whitespace/Null/WrongFormat)
GetAllPropertiesByVersion_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
GetAllPropertiesByVersion_ShouldHandleCancellationToken
```

---


# 🟧 **StylesController – `api/styles`**

---

## Commands

---

### **POST `/api/styles`**

**Purpose:** Creates a new style.
**Results:** 

* 201 Created
* 409 Conflict
* 400 BadRequest

```
Update_ShouldReturnOk_WhenStyleUpdated
Update_ShouldReturnNotFound_WhenStyleDoesNotExist
Update_ShouldReturnBadRequest_WhenRequestIsInvalid (NameInvalid/TypeInvalid/TagsInvalid/DescriptionInvalid)
Update_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
Update_ShouldHandleCancellationToken

```

---

### **POST `/api/styles/{name}/tags/{tag}`**

**Purpose:** Adds a tag to the selected style.
**Results:** 

* 200 OK
* 404 NotFound
* 409 Conflict
* 400 BadRequest

```
AddTag_ShouldReturnOk_WhenTagAddedToStyle
AddTag_ShouldReturnNotFound_WhenStyleDoesNotExist
AddTag_ShouldReturnConflict_WhenTagAlreadyExistsInStyle
AddTag_ShouldReturnBadRequest_WhenNameIsInvalid (TooLong/Whitespace/Null)
AddTag_ShouldReturnBadRequest_WhenTagIsInvalid (TooLong/Whitespace/Null)
AddTag_ShouldReturnBadRequest_WhenNameAndTagAreInvalid (TooLong/Whitespace/Null)
AddTag_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / SomeMore)
AddTag_ShouldHandleCancellationToken
```

---

### **DELETE `/api/styles/{name}`**

**Purpose:** Deletes a style by its name.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
Delete_ShouldReturnOk_WhenStyleDeleted
Delete_ShouldReturnNotFound_WhenStyleDoesNotExist
Delete_ShouldReturnBadRequest_WhenNameIsInvalid (TooLong/Whitespace/Null)
Delete_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
Delete_ShouldHandleCancellationToken
```

---

### **DELETE `/api/styles/{name}/tags/{tag}`**

**Purpose:** Removes a tag from the selected style.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
RemoveTag_ShouldReturnOk_WhenTagRemoved
RemoveTag_ShouldReturnNotFound_WhenStyleDoesNotExist
RemoveTag_ShouldReturnNotFound_WhenTagDoesNotExist
RemoveTag_ShouldReturnBadRequest_WhenNameIsInvalid (TooLong/Whitespace/Null)
RemoveTag_ShouldReturnBadRequest_WhenTagIsInvalid (TooLong/Whitespace/Null)
RemoveTag_ShouldReturnBadRequest_WhenNameAndTagAreInvalid (TooLong/Whitespace/Null)
RemoveTag_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
RemoveTag_ShouldHandleCancellationToken

```

---

### **PUT `/api/styles/{name}/description`**

**Purpose:** Updates the description of a style.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
UpdateDescription_ShouldReturnOk_WhenDescriptionUpdated
UpdateDescription_ShouldReturnNotFound_WhenStyleDoesNotExist
UpdateDescription_ShouldReturnBadRequest_WhenDescriptionIsInvalid (TooLong/Whitespace)
UpdateDescription_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
UpdateDescription_ShouldHandleCancellationToken
```

---

### **PUT `/api/styles`**

**Purpose:** Updates an existing style (full update).
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
Update_ShouldReturnOk_WhenStyleUpdated
Update_ShouldReturnNotFound_WhenStyleDoesNotExist
Update_ShouldReturnBadRequest_WhenRequestIsInvalid (NameInvalid/TypeInvalid/TagsInvalid/DescriptionInvalid)
Update_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
Update_ShouldHandleCancellationToken
```

---

## Queries

---

### **GET `/api/styles/{name}/exists`**

**Purpose:** Checks if a style with the given name exists.
**Results:** 

* 200 OK
* 400 BadRequest

```
CheckExists_ShouldReturnTrue_WhenStyleExists
CheckExists_ShouldReturnFalse_WhenStyleDoesNotExist
CheckExists_ShouldReturnBadRequest_WhenNameIsInvalid (TooLong/Whitespace/Null)
CheckExists_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore)
CheckExists_ShouldHandleCancellationToken

```

---

### **GET `/api/styles/{name}/tags/{tag}/exists`**

**Purpose:** Checks whether a style contains a given tag.
**Results:** 

* 200 OK
* 400 BadRequest

```
CheckTagExists_ShouldReturnTrue_WhenTagExistsInStyle
CheckTagExists_ShouldReturnFalse_WhenTagDoesNotExistInStyle
CheckTagExists_ShouldReturnBadRequest_WhenStyleNameIsInvalid (TooLong/Whitespace/Null)
CheckTagExists_ShouldReturnBadRequest_WhenTagIsInvalid (TooLong/Whitespace/Null)
CheckTagExists_ShouldReturnBadRequest_WhenStyleNameAndTagIsInvalid (TooLong/Whitespace/Null)
CheckTagExists_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore)
CheckTagExists_ShouldHandleCancellationToken
```

### **GET `/api/styles`**

**Purpose:** Returns all styles.
**Results:** 

* 200 OK

```
GetAll_ShouldReturnEmptyList_WhenThereIsNoData
GetAll_ShouldReturnListOfStyles_WhenDataExists
GetAll_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / SomeMore)
GetAll_ShouldHandleCancellationToken
GetAll_ShouldUseSingletonQuery

```

---

### **GET `/api/styles/tags`**

**Purpose:** Returns all style tags available in the system.
**Results:**

* 200 OK — list of tags

```
GetAllTags_ShouldReturnEmptyList_WhenThereIsNoData 
GetAllTags_ShouldReturnList_WhenTagsExist 
GetAllTags_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / SomeMore) 
GetAllTags_ShouldHandleCancellationToken 
GetAllTags_ShouldUseSingletonQuery
```

---

### **GET `/api/styles/{name}`**

**Purpose:** Returns a style by name.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
GetByName_ShouldReturnStyle_WhenStyleExists
GetByName_ShouldReturnNotFound_WhenStyleDoesNotExist
GetByName_ShouldReturnBadRequest_WhenNameIsInvalid (TooLong/Whitespace/Null)
GetByName_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
GetByName_ShouldHandleCancellationToken
```

---

### **GET `/api/styles/by-description`**

**Purpose:** Returns styles where description contains a keyword.
**Results:** 

* 200 OK
* 400 BadRequest

```
GetByDescription_ShouldReturnList_WhenKeywordMatchesDescriptions
GetByDescription_ShouldReturnEmptyList_WhenNoDescriptionsMatch
GetByDescription_ShouldReturnBadRequest_WhenKeywordIsInvalid (TooLong/Whitespace/Null)
GetByDescription_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore)
GetByDescription_ShouldHandleCancellationToken
```

---

### **GET `/api/styles/by-tags?tags=A&tags=B`**

**Purpose:** Returns styles that contain ALL provided tags.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
GetByTags_ShouldReturnList_WhenAllTagsMatchStyles
GetByTags_ShouldReturnEmptyList_WhenNoStylesMatchTags
GetByTags_ShouldReturnNotFound_WhenOneOrMoreTagsDoNotExist
GetByTags_ShouldReturnBadRequest_WhenNoTagsProvided
GetByTags_ShouldReturnBadRequest_WhenToManyTagsProvided
GetByTags_ShouldReturnBadRequest_WhenOneOrMoreTagsIsInvalid (TooLong/Whitespace/Null)
GetByTags_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore)
GetByTags_ShouldHandleCancellationToken

```

---

### **GET `/api/styles/by-type/{type}`**

**Purpose:** Returns all styles belonging to a specific type.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
GetByType_ShouldReturnList_WhenTypeExistsAndHasStyles
GetByType_ShouldReturnEmptyList_WhenTypeExistsButNoStyles (!!)
GetByType_ShouldReturnNotFound_WhenTypeDoesNotExist
GetByType_ShouldReturnBadRequest_WhenTypeIsInvalid (TooLong/Whitespace/Null)
GetByType_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore)
GetByType_ShouldHandleCancellationToken
```

---


# 🟥 **VersionsController – `api/versions`**

---

## Commands

---

### **POST `/api/versions`**

**Purpose:** Creates a new version.
**Results:** 

* 201 Created
* 409 Conflict
* 400 BadRequest

```
Create_ShouldReturnCreated_WhenVersionIsAdded
Create_ShouldReturnConflict_WhenVersionAlreadyExists
Create_ShouldReturnBadRequest_WhenVersionIsInvalid (TooLong/WrongFormat/Null/Whitespace)
Create_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (200 / 404 / SomeMore)
Create_ShouldReturnLocationHeaderPointingToNewVersion
Create_ShouldHandleCancellationToken
```

---

## Queries

---

### **GET `/api/versions/no-empty`**

**Purpose:** Checks whether at least one version exists (global existence check).
**Results:**

* 200 OK — boolean

```
CheckAnyExists_ShouldReturnTrue_WhenAnyVersionExists
CheckAnyExists_ShouldReturnFalse_WhenNoVersionsExist
CheckAnyExists_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / SomeMore)
CheckAnyExists_ShouldHandleCancellationToken
CheckAnyExists_ShouldUseSingletonQuery
```

---

### **GET `/api/versions/{version}/exists`**

**Purpose:** Checks whether a version exists.
**Results:** 

* 200 OK
* 400 BadRequest

```
CheckExists_ShouldReturnTrue_WhenVersionExists
CheckExists_ShouldReturnFalse_WhenVersionDoesNotExist
CheckExists_ShouldReturnBadRequest_WhenVersionIsInvalid (TooLong/Whitespace/Null/WrongFormat)
CheckExists_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore)
CheckExists_ShouldHandleCancellationToken
```

---

### **GET `/api/versions/supported`**

**Purpose:** Returns all versions marked as *supported*.
**Results:**

* 200 OK

```
GetAll_ShouldReturnEmptyList_WhenThereIsNoSuportedVersions
GetAll_ShouldReturnListOfVersions_WhenSuportedVersionsExists
GetAll_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / SomeMore)
GetAll_ShouldHandleCancellationToken
GetAll_ShouldUseSingletonQuery
```

---

### **GET `/api/versions`**

**Purpose:** Returns all versions defined in the system.
**Results:** 

* 200 OK

```
GetAll_ShouldReturnEmptyList_WhenThereIsNoData
GetAll_ShouldReturnListOfVersions_WhenDataExists
GetAll_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 400 / 409 / SomeMore)
GetAll_ShouldHandleCancellationToken
GetAll_ShouldUseSingletonQuery
```

---

### **GET `/api/versions/latest`**

**Purpose:** Returns the latest version in the system.
**Results:**

* 200 OK
* 400 BadRequest

```
GetLatest_ShouldReturnVersion_WhenVersionsExist 
GetLatest_ShouldReturnBadRequest_WhenNoVersionsExist 
GetLatest_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 404 / 409 / SomeMore) 
GetLatest_ShouldHandleCancellationToken 
GetLatest_ShouldUseSingletonQuery
```

---

### **GET `/api/versions/{version}`**

**Purpose:** Returns details for a specific version.
**Results:** 

* 200 OK
* 404 NotFound
* 400 BadRequest

```
GetByVersion_ShouldReturnVersion_WhenVersionExists
GetByVersion_ShouldReturnNotFound_WhenVersionDoesNotExist
GetByVersion_ShouldReturnBadRequest_WhenVersionIsInvalid (TooLong/Whitespace/Null/WrongFormat)
GetByVersion_ShouldThrowExceptionWithProblemDetail_WhenUnexpectedResponseOccurs (201 / 409 / SomeMore)
GetByVersion_ShouldHandleCancellationToken
```

---






