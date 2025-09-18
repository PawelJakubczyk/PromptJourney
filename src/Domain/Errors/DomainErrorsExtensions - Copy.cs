//using Domain.Abstractions;
//using Domain.ValueObjects;
//using FluentResults;

//namespace Domain.Errors;

//public static class DomainErrorsExtensionsOld
//{
//    public static List<DomainError> IfNullOrWhitespace<TValue>(this List<DomainError> domainErrors, string value)
//    {
//        if (string.IsNullOrWhiteSpace(value))
//        {
//            domainErrors.Add(new DomainError($"{typeof(TValue).Name}: {value} cannot be null or whitespace."));
//        }

//        return domainErrors;
//    }

//    public static List<DomainError> IfWhitespace<TValue>(this List<DomainError> domainErrors, string value)
//    {
//        if (string.IsNullOrWhiteSpace(value) && value != null)
//        {
//            domainErrors.Add(new DomainError($"{typeof(TValue).Name}: cannot be whitespace."));
//        }

//        return domainErrors;
//    }

//    public static List<DomainError> IfNull<TValue>(this List<DomainError> domainErrors, object? value)
//    {
//        if (value is null)
//        {
//            domainErrors.Add(new DomainError($"{typeof(TValue).Name}: cannot be null."));
//        }
//        return domainErrors;
//    }

//    public static List<DomainError> IfLengthTooLong<TValue>(this List<DomainError> domainErrors, string value, int maxLength)
//        where TValue : ValueObject<string>
//    {
//        if (value?.Length > maxLength)
//        {
//            domainErrors.Add(new DomainError($"{typeof(TValue).Name}: {value} cannot be longer than {maxLength} characters."));
//        }

//        return domainErrors;
//    }

//    public static List<DomainError> IfListIsEmpty<TValue>(this List<DomainError> domainErrors, List<TValue>? items)
//    {
//        if (items != null && items.Count == 0)
//        {
//            domainErrors.Add(new DomainError($"{typeof(TValue).Name}: Cannot be an empty collection."));
//        }
//        return domainErrors;
//    }

//    public static List<DomainError> IfDoesNotContain<TValue>(this List<DomainError> domainErrors, List<TValue>? items, TValue element)
//    {
//        if (items != null && items.Contains(element) == false)
//        {
//            domainErrors.Add(new DomainError($"{typeof(TValue).Name}: Collection does not contain the required element."));
//        }
//        return domainErrors;
//    }

//    public static List<DomainError> IfTagAllredyExist(this List<DomainError> domainErrors, List<Tag>? items, Tag element)
//    {
//        if (items != null && items.Contains(element))
//        {
//            domainErrors.Add(new DomainError($"tag: {element} already exist in tags: {items}."));
//        }
//        return domainErrors;
//    }

//    public static List<DomainError> IfContain<TValue>(this List<DomainError> domainErrors, List<TValue>? items, TValue element)
//    {
//        if (items != null && items.Contains(element))
//        {
//            domainErrors.Add(new DomainError($"{typeof(TValue).Name}: Collection already contains the element."));
//        }
//        return domainErrors;
//    }

//    public static List<DomainError> CollectErrors<TValue>(
//        this List<DomainError> errors,
//        Result<TValue>? result)
//    {
//        if (result is not null && result.IsFailed)
//        {
//            errors.AddRange(result.Errors.OfType<DomainError>());
//        }

//        return errors;
//    }

//}