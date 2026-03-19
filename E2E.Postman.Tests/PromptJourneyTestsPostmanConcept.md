# PromptJourney - Lista testów Postman

## 📋 Spis treści
- [Versions](#versions) - **45 testów** ✅
- [Styles](#styles) - **31 testów** ✅
- [Properties](#properties) - **34 testy** 🆕
- [ExampleLinks](#examplelinks) - **28 testów** 🆕
- [PromptHistory](#prompthistory) - **26 testów** 🆕

**Łącznie: 164 testy (76 istniejących + 88 nowych)**

---

## Versions (45 testów)

### Create (24 testy)
1. `Create_ShouldReturnCreated_WhenVersionIsAdded`

2. `Create_ShouldReturnCreated_WhenNijiVersionIsValid`
3. `Create_ShouldReturnCreated_WhenOnlyRequiredFieldsAreProvided`
4. `Create_ShouldIgnoreUnknownFields_WhenAdditionalFieldIsProvided`

5. `Create_ShouldReturnConflict_WhenVersionAlreadyExists`
6. `Create_ShouldReturnConflict_WhenParameterAlreadyExistsWithDifferentVersion`

7. `Create_ShouldReturnBadRequest_WhenVersionIsInvalid_TooLong`
8. `Create_ShouldReturnBadRequest_WhenVersionIsInvalid_WrongFormat`
9. `Create_ShouldReturnBadRequest_WhenVersionIsInvalid_TooLong_WrongFormat`
10. `Create_ShouldReturnBadRequest_WhenVersionIsInvalid_Whitespace`
11. `Create_ShouldReturnBadRequest_WhenVersionIsInvalid_Null`

12. `Create_ShouldReturnBadRequest_WhenVersionIsMissing`
13. `Create_ShouldReturnBadRequest_WhenParameterIsMissing`
14. `Create_ShouldReturnBadRequest_WhenReleaseDateIsMissing`
15. `Create_ShouldReturnBadRequest_WhenPayloadIsEmpty`

16. `Create_ShouldReturnBadRequest_WhenDescriptionIsTooLong`
17. `Create_ShouldReturnBadRequest_WhenDescriptionContainsXssPayload`

18. `Create_ShouldReturnBadRequest_WhenParameterIsInvalidFormat_ButVersionIsValid`
19. `Create_ShouldReturnBadRequest_WhenParameterIsTooLong_ButVersionIsValid`

20. `Create_ShouldReturnBadRequest_WhenReleaseDateHasInvalidFormat`

21. `Create_ShouldReturnBadRequest_WhenAllFieldsAreInvalid`

22. `Create_ShouldReturnUnsupportedMediaType_WhenContentTypeIsInvalid`

23. `Create_ShouldReturnBadRequest_WhenJsonIsMalformed` ⚠️ *(Skipped)*

### Exists (7 testów)
24. `CheckExists_ShouldReturnTrue_WhenVersionExists`
25. `CheckExists_ShouldReturnTrue_WhenNijiVersionExists`

26. `CheckExists_ShouldReturnFalse_WhenVersionDoesNotExist`

27. `CheckExists_ShouldReturnBadRequest_WhenVersionIsInvalid_TooLong`
28. `CheckExists_ShouldReturnBadRequest_WhenVersionIsInvalid_WrongFormat`
29. `CheckExists_ShouldReturnBadRequest_WhenVersionIsInvalid_Whitespace`
30. `CheckExists_ShouldReturnBadRequest_WhenVersionIsInvalid_Null`

### GetAll (3 testy)
31. `GetAll_ShouldReturnListOfVersions_WhenDataExists`
32. `GetAll_ShouldReturnListSortedByVersion_WhenDefaultSortingIsApplied`

33. `GetAll_ShouldReturnEmptyList_WhenThereIsNoData`

### GetLatest (3 testy)
34. `GetLatest_ShouldReturnVersion_WhenVersionsExist`
35. `GetLatest_ShouldReturnHighestDateVersion_WhenMultipleVersionsExist`

36. `GetLatest_ShouldReturnNotFound_WhenNoVersionsExist`

### Supported (2 testy)
37. `GetSupported_ShouldReturnListOfSupportedVersions_WhenSupportedExists`

38. `GetSupported_ShouldReturnEmptyList_WhenNoSupportedVersionsExist`

### CheckAnyExists (2 testy)
39. `CheckAnyExists_ShouldReturnTrue_WhenAnyVersionExists`

40. `CheckAnyExists_ShouldReturnFalse_WhenNoVersionsExist`

### GetByVersion (3 testy)
41. `GetByVersion_ShouldReturnVersion_WhenVersionExists`

42. `GetByVersion_ShouldReturnNotFound_WhenVersionDoesNotExist`

43. `GetByVersion_ShouldReturnBadRequest_WhenVersionIsInvalid_TooLong`
44. `GetByVersion_ShouldReturnBadRequest_WhenVersionIsInvalid_WrongFormat`
45. `GetByVersion_ShouldReturnBadRequest_WhenVersionIsInvalid_Whitespace`
46. `GetByVersion_ShouldReturnBadRequest_WhenVersionIsInvalid_Null`

### Delete (2 testy) 🆕

47. `Delete_ShouldReturnNoContent_WhenVersionDeleted`

48. `Delete_ShouldReturnNotFound_WhenVersionDoesNotExist`

49. `Delete_ShouldReturnBadRequest_WhenVersion_TooLong`
49. `Delete_ShouldReturnBadRequest_WhenVersion_WrongFormat`
49. `Delete_ShouldReturnBadRequest_WhenVersion_Whitespace`
49. `Delete_ShouldReturnBadRequest_WhenVersion_Null`
---

## Styles (31 testów)

### Create (7 testów)
1. `Create_ShouldReturnCreated_WhenStyleIsValid`

2. `Create_ShouldReturnCreated_WhenDescriptionIsMissing`
2. `Create_ShouldReturnCreated_WhenDescriptionIsEmpty`
1. `Create_ShouldReturnCreated_WhenDescriptionIsNull`
3. `Create_ShouldReturnCreated_WhenTagsAreMissing`
3. `Create_ShouldReturnCreated_WhenTagsIsEmpty`
3. `Create_ShouldReturnCreated_WhenTagsIsNull`
3. `Create_ShouldReturnCreated_WhenOnlyRequiredFieldsAreProvided`
4. `Create_ShouldIgnoreUnknownFields_WhenAdditionalFieldIsProvided`

2. `Create_ShouldReturnConflict_WhenStyleAlreadyExists`

3. `Create_ShouldReturnBadRequest_WhenNameIsNull`
4. `Create_ShouldReturnBadRequest_WhenNameIsTooLong`
5. `Create_ShouldReturnBadRequest_WhenTypeIsInvalidNotInclude`
5. `Create_ShouldReturnBadRequest_WhenTypeIsNull`
5. `Create_ShouldReturnBadRequest_WhenTypeIsWhitespace`
5. `Create_ShouldReturnBadRequest_WhenDescriptionIsTooLong`
6. `Create_ShouldReturnBadRequest_WhenOneOfTagsIsNull`
1. `Create_ShouldReturnBadRequest_WhenOneOfTagsIsWhitespace`
1. `Create_ShouldReturnBadRequest_WhenOneOfTagsIsTooLong`

7. `Create_ShouldReturnBadRequest_WhenJsonIsMalformed` ⚠️ *(Skipped)*

### Update (1 test)
8. `Update_ShouldReturnOk_WhenStyleUpdated`

### GetAll (2 testy)
9. `GetAll_ShouldReturnOkWithList_WhenStylesExist`

10. `GetAll_ShouldReturnOkWithEmptyList_WhenNoStylesExist`

### GetByName (4 testy)
11. `GetByName_ShouldReturnOk_WhenStyleExists`

12. `GetByName_ShouldReturnNotFound_WhenStyleDoesNotExist`

13. `GetByName_ShouldReturnBadRequest_WhenStyleNameToLong`
14. `GetByName_ShouldReturnBadRequest_WhenStyleNameIsNull`

### GetByType (5 testów)
15. `GetByType_ShouldReturnOkWithList_WhenStylesOfGivenTypeExist`

16. `GetByType_ShouldReturnEmptyList_WhenNoStylesOfGivenTypeExist`

17. `GetByType_ShouldReturnBadRequest_WhenTypeIsInvalid`
18. `GetByType_ShouldReturnBadRequest_WhenTypeIsNull`
19. `GetByType_ShouldReturnBadRequest_WhenTypeIsWhiteSpace`

### CheckExists (5 testów)
20. `CheckExists_ShouldReturnTrue_WhenStyleExists`

21. `CheckExists_ShouldReturnFalse_WhenStyleDoesNotExist`

22. `CheckExists_ShouldReturnBadRequest_WhenNameIsTooLong`
23. `CheckExists_ShouldReturnBadRequest_WhenNameIsWhitespace`
24. `CheckExists_ShouldReturnBadRequest_WhenNameIsNull`

### GetByTags (7 testów)
25. `GetByTags_ShouldReturnOkWithList_WhenStylesWithAllTagsExist`
26. `GetByTags_ShouldReturnOkWithList_WhenStylesWithSingleTagExist`

27. `GetByTags_ShouldReturnEmptyList_WhenNoStylesMatchTags`

28. `GetByTags_ShouldReturnBadRequest_WhenTagsQueryIsInvalidTagNull`
29. `GetByTags_ShouldReturnBadRequest_WhenTagsIsInvalidNull`
30. `GetByTags_ShouldReturnBadRequest_WhenTagsIsInvalidWhitespace`
31. `GetByTags_ShouldReturnBadRequest_WhenTagsIsInvalidTooLong`

---

## Properties (34 testy) 🆕

### Create (15 testów)
1. `Create_ShouldReturnCreated_WhenPropertyIsValid`
2. `Create_ShouldReturnCreated_WhenOptionalFieldsAreMissing`
3. `Create_ShouldReturnCreated_WhenDescriptionIsMissing`

4. `Create_ShouldReturnConflict_WhenPropertyAlreadyExists`

5. `Create_ShouldReturnBadRequest_WhenPropertyNameIsNull`
6. `Create_ShouldReturnBadRequest_WhenPropertyNameIsTooLong`
7. `Create_ShouldReturnBadRequest_WhenPropertyNameIsWhitespace`

8. `Create_ShouldReturnBadRequest_WhenVersionIsInvalid_TooLong`
9. `Create_ShouldReturnBadRequest_WhenVersionIsInvalid_WrongFormat`
10. `Create_ShouldReturnBadRequest_WhenVersionDoesNotExist`

11. `Create_ShouldReturnBadRequest_WhenParametersCollectionIsEmpty`
12. `Create_ShouldReturnBadRequest_WhenParametersContainInvalidFormat`

13. `Create_ShouldReturnBadRequest_WhenDefaultValueIsTooLong`
14. `Create_ShouldReturnBadRequest_WhenMinValueIsTooLong`
15. `Create_ShouldReturnBadRequest_WhenMaxValueIsTooLong`

### Update (5 testów)
16. `Update_ShouldReturnOk_WhenPropertyUpdated`
17. `Update_ShouldReturnOk_WhenPartialFieldsUpdated`

18. `Update_ShouldReturnNotFound_WhenPropertyDoesNotExist`

19. `Update_ShouldReturnBadRequest_WhenDescriptionIsTooLong`
20. `Update_ShouldReturnBadRequest_WhenParametersAreInvalid`

### Patch (3 testy)
21. `Patch_ShouldReturnOk_WhenSingleFieldPatched`
22. `Patch_ShouldReturnOk_WhenMultipleFieldsPatched`

23. `Patch_ShouldReturnNotFound_WhenPropertyDoesNotExist`

### GetAllByVersion (3 testy)
24. `GetAllByVersion_ShouldReturnOkWithList_WhenPropertiesExist`

25. `GetAllByVersion_ShouldReturnEmptyList_WhenNoPropertiesForVersion`

26. `GetAllByVersion_ShouldReturnBadRequest_WhenVersionIsInvalid`

### GetByPropertyNameAndVersion (3 testy)
27. `GetByNameAndVersion_ShouldReturnOk_WhenPropertyExists`

28. `GetByNameAndVersion_ShouldReturnNotFound_WhenPropertyDoesNotExist`

29. `GetByNameAndVersion_ShouldReturnBadRequest_WhenVersionIsInvalid`

### CheckExists (3 testy)
30. `CheckExists_ShouldReturnTrue_WhenPropertyExists`

31. `CheckExists_ShouldReturnFalse_WhenPropertyDoesNotExist`

32. `CheckExists_ShouldReturnBadRequest_WhenParametersAreInvalid`

### Delete (2 testy)
33. `Delete_ShouldReturnNoContent_WhenPropertyDeleted`

34. `Delete_ShouldReturnNotFound_WhenPropertyDoesNotExist`

---

## ExampleLinks (28 testów) 🆕

### Create (12 testów)
1. `Create_ShouldReturnCreated_WhenExampleLinkIsValid`

2. `Create_ShouldReturnConflict_WhenExampleLinkAlreadyExists`

3. `Create_ShouldReturnBadRequest_WhenLinkIsNull`
4. `Create_ShouldReturnBadRequest_WhenLinkIsTooLong`
5. `Create_ShouldReturnBadRequest_WhenLinkIsInvalidFormat`
6. `Create_ShouldReturnBadRequest_WhenLinkIsNotHttpOrHttps`

7. `Create_ShouldReturnBadRequest_WhenStyleNameIsNull`
8. `Create_ShouldReturnBadRequest_WhenStyleNameDoesNotExist`

9. `Create_ShouldReturnBadRequest_WhenVersionIsNull`
10. `Create_ShouldReturnBadRequest_WhenVersionIsInvalidFormat`
11. `Create_ShouldReturnBadRequest_WhenVersionDoesNotExist`

12. `Create_ShouldReturnBadRequest_WhenAllFieldsAreInvalid`

### GetAll (2 testy)
13. `GetAll_ShouldReturnOkWithList_WhenExampleLinksExist`

14. `GetAll_ShouldReturnEmptyList_WhenNoExampleLinksExist`

### GetByStyleAndVersion (3 testy)
15. `GetByStyleAndVersion_ShouldReturnOkWithList_WhenLinksExist`

16. `GetByStyleAndVersion_ShouldReturnEmptyList_WhenNoLinksForStyleAndVersion`

17. `GetByStyleAndVersion_ShouldReturnBadRequest_WhenParametersAreInvalid`

### GetByStyle (3 testy)
18. `GetByStyle_ShouldReturnOkWithList_WhenLinksExistForStyle`

19. `GetByStyle_ShouldReturnEmptyList_WhenNoLinksForStyle`

20. `GetByStyle_ShouldReturnBadRequest_WhenStyleNameIsInvalid`

### CheckExists (3 testy)
21. `CheckLinkExists_ShouldReturnTrue_WhenLinkExists`

22. `CheckLinkExists_ShouldReturnFalse_WhenLinkDoesNotExist`

23. `CheckLinkExists_ShouldReturnBadRequest_WhenLinkIdIsInvalid`

### CheckLinkWithStyleExists (2 testy)
24. `CheckLinkWithStyleExists_ShouldReturnTrue_WhenLinkExistsForStyle`

25. `CheckLinkWithStyleExists_ShouldReturnFalse_WhenLinkDoesNotExistForStyle`

### CheckLinksEmpty (1 test)
26. `CheckLinksEmpty_ShouldReturnTrue_WhenNoLinksExist`

### Delete (2 testy)
27. `Delete_ShouldReturnNoContent_WhenExampleLinkDeleted`

28. `Delete_ShouldReturnNotFound_WhenExampleLinkDoesNotExist`

---

## PromptHistory (26 testów) 🆕

### Create (10 testów)
1. `Create_ShouldReturnCreated_WhenPromptHistoryIsValid`

2. `Create_ShouldReturnBadRequest_WhenPromptIsNull`
3. `Create_ShouldReturnBadRequest_WhenPromptIsWhitespace`
4. `Create_ShouldReturnBadRequest_WhenPromptIsTooLong`
5. `Create_ShouldReturnBadRequest_WhenPromptContainsXssPayload`

6. `Create_ShouldReturnBadRequest_WhenVersionIsNull`
7. `Create_ShouldReturnBadRequest_WhenVersionIsInvalidFormat`
8. `Create_ShouldReturnBadRequest_WhenVersionDoesNotExist`

9. `Create_ShouldReturnBadRequest_WhenStylesListContainsNonExistentStyle`

10. `Create_ShouldReturnUnsupportedMediaType_WhenContentTypeIsInvalid`

### GetAll (2 testy)
11. `GetAll_ShouldReturnOkWithList_WhenPromptHistoriesExist`

12. `GetAll_ShouldReturnEmptyList_WhenNoPromptHistoriesExist`

### GetByDateRange (4 testy)
13. `GetByDateRange_ShouldReturnOkWithList_WhenPromptHistoriesExistInRange`

14. `GetByDateRange_ShouldReturnEmptyList_WhenNoPromptHistoriesInRange`

15. `GetByDateRange_ShouldReturnBadRequest_WhenStartDateIsInvalidFormat`
16. `GetByDateRange_ShouldReturnBadRequest_WhenEndDateIsBeforeStartDate`

### GetByKeyword (4 testy)
17. `GetByKeyword_ShouldReturnOkWithList_WhenPromptHistoriesMatchKeyword`

18. `GetByKeyword_ShouldReturnEmptyList_WhenNoPromptHistoriesMatchKeyword`

19. `GetByKeyword_ShouldReturnBadRequest_WhenKeywordIsNull`
20. `GetByKeyword_ShouldReturnBadRequest_WhenKeywordIsTooLong`

### GetLast (3 testy)
21. `GetLast_ShouldReturnOk_WhenPromptHistoriesExist`
22. `GetLast_ShouldReturnOkWithSpecifiedCount_WhenCountParameterProvided`

23. `GetLast_ShouldReturnEmptyList_WhenNoPromptHistoriesExist`

### GetRecordCount (2 testy)
24. `GetRecordCount_ShouldReturnOkWithCount_WhenPromptHistoriesExist`

25. `GetRecordCount_ShouldReturnZero_WhenNoPromptHistoriesExist`

### Delete (1 test)
26. `Delete_ShouldReturnNoContent_WhenPromptHistoryDeleted`

---
