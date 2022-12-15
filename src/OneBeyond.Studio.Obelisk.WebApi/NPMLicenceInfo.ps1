param (
    [Parameter(Mandatory="true", Position=0, ValueFromPipeline=$false)]
    [String]
    $OutputPath
);

$obj = New-Object -TypeName PSObject;

$(get-content package.json | convertfrom-json).dependencies |
Get-Member -MemberType NoteProperty |
Where-Object { $_.Name -notlike '*waterloo*'} |

ForEach-Object {
    $package=$_.Name; 
    $licence_path="./node_modules/$package/LICEN?E*";
    $package_definition = "$($_.Definition.split()[1] -replace "=", " ")";

    if (Test-Path $licence_path -PathType leaf) {
        $licence = "$($(Get-Content $licence_path -First 1).Trim())";
        $content = "$($(Get-Content $licence_path -raw))";

        if ($licence -Like '*licen?e*') {
            if ($licence -Like '*mit*') {
                $obj | Add-Member -MemberType NoteProperty -Name "$package_definition - MIT Licence" -Value "$content";
            }
            else {
                $obj | Add-Member -MemberType NoteProperty -Name "$package_definition - $licence" -Value "$content"; 
            }
            return;
        }
    }
    else {
        $obj | Add-Member -MemberType NoteProperty -Name "$package_definition - Non-Standard Licence" -Value "No licence information available.";  
    }
}

$obj | ConvertTo-Json | Out-File "$OutputPath"