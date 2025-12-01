# Skrypt do automatycznego formatowania asercji w testach
# Uruchom w katalogu g³ównym projektu

$testFolder = "test\Unit.Presentation.Tests\MoqControlersTests\ExampleLinksMoqControlersTests"
$files = Get-ChildItem -Path $testFolder -Filter "*.cs" -File

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Wzorzec 1: actionResult.Should().BeBadRequestResult().WithMessage(...)
    # Zamieñ na format z enterami
    if ($content -match 'actionResult\.Should\(\)\.BeBadRequestResult\(\)\.WithMessage\(') {
        $content = $content -replace `
            'actionResult\.Should\(\)\.BeBadRequestResult\(\)\.WithMessage\(([^)]+)\)',
            "actionResult`n            .Should()`n            .BeBadRequestResult()`n            .WithMessage(`$1)"
        $modified = $true
    }
    
    # Wzorzec 2: actionResult.Should().BeNotFoundResult().WithMessage(...)
    if ($content -match 'actionResult\.Should\(\)\.BeNotFoundResult\(\)\.WithMessage\(') {
        $content = $content -replace `
            'actionResult\.Should\(\)\.BeNotFoundResult\(\)\.WithMessage\(([^)]+)\)',
            "actionResult`n            .Should()`n            .BeNotFoundResult()`n            .WithMessage(`$1)"
        $modified = $true
    }
    
    # Wzorzec 3: actionResult.Should().BeConflictResult().WithMessage(...)
    if ($content -match 'actionResult\.Should\(\)\.BeConflictResult\(\)\.WithMessage\(') {
        $content = $content -replace `
            'actionResult\.Should\(\)\.BeConflictResult\(\)\.WithMessage\(([^)]+)\)',
            "actionResult`n            .Should()`n            .BeConflictResult()`n            .WithMessage(`$1)"
        $modified = $true
    }
    
    # Wzorzec 4: actionResult.Should().BeOkResult().WithValueOfType<...>()
    if ($content -match 'actionResult\.Should\(\)\.BeOkResult\(\)\.WithValueOfType') {
        $content = $content -replace `
            'actionResult\.Should\(\)\.BeOkResult\(\)\.WithValueOfType<([^>]+)>\(\)',
            "actionResult`n            .Should()`n            .BeOkResult()`n            .WithValueOfType<`$1>()"
        $modified = $true
    }
    
    # Wzorzec 5: actionResult.Should().BeOkResult().WithCount(...)
    if ($content -match 'actionResult\.Should\(\)\.BeOkResult\(\)\.WithCount') {
        $content = $content -replace `
            'actionResult\.Should\(\)\.BeOkResult\(\)\.WithCount\(([^)]+)\)',
            "actionResult`n            .Should()`n            .BeOkResult()`n            .WithCount(`$1)"
        $modified = $true
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Zaktualizowano: $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "Pominiêto: $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host "`nGotowe!" -ForegroundColor Cyan
