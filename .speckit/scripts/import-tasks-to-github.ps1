# Import Tasks to GitHub Project
# This script creates GitHub Issues from the CSV file and adds them to a GitHub Project Board
# Prerequisites: 
# - GitHub CLI installed: winget install GitHub.cli
# - Authenticated: gh auth login

param(
    [string]$CsvPath = ".speckit/tasks/santa-api-tasks.csv",
    [string]$Owner = "Ace68",
    [string]$Repo = "Spec-DrivenDevelopment",
    [string]$ProjectName = "Santa API Implementation",
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

# Check if GitHub CLI is installed
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh) is not installed. Install it with: winget install GitHub.cli"
    exit 1
}

# Check if authenticated
$authStatus = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Not authenticated with GitHub. Run: gh auth login"
    exit 1
}

Write-Host "✓ GitHub CLI is installed and authenticated" -ForegroundColor Green

# Read CSV file
if (-not (Test-Path $CsvPath)) {
    Write-Error "CSV file not found: $CsvPath"
    exit 1
}

$tasks = Import-Csv -Path $CsvPath
Write-Host "✓ Loaded $($tasks.Count) tasks from CSV" -ForegroundColor Green

# Map status to project column
function Get-ProjectStatus($status) {
    switch ($status) {
        "Ready" { return "Todo" }
        "Blocked" { return "Todo" }
        "In Progress" { return "In Progress" }
        "Complete" { return "Done" }
        default { return "Todo" }
    }
}

# Map priority to label
function Get-PriorityLabel($priority) {
    switch ($priority) {
        "Critical" { return "priority: critical" }
        "High" { return "priority: high" }
        "Medium" { return "priority: medium" }
        default { return "priority: low" }
    }
}

if ($DryRun) {
    Write-Host "`n=== DRY RUN MODE - No changes will be made ===" -ForegroundColor Yellow
    Write-Host "`nWould create $($tasks.Count) issues with the following labels:" -ForegroundColor Cyan
    
    $tasks | Select-Object -First 5 | ForEach-Object {
        Write-Host "`n$($_.'Task ID'): $($_.Title)" -ForegroundColor White
        Write-Host "  Phase: $($_.Phase)" -ForegroundColor Gray
        Write-Host "  Priority: $($_.Priority)" -ForegroundColor Gray
        Write-Host "  Status: $($_.Status) -> $(Get-ProjectStatus $_.Status)" -ForegroundColor Gray
        Write-Host "  Dependencies: $($_.Dependencies)" -ForegroundColor Gray
    }
    Write-Host "`n... and $($tasks.Count - 5) more tasks" -ForegroundColor Gray
    Write-Host "`nRun without -DryRun to create issues and project" -ForegroundColor Yellow
    exit 0
}

Write-Host "`n=== Creating Labels ===" -ForegroundColor Cyan

# Create labels if they don't exist
$labels = @(
    @{name="priority: critical"; color="d73a4a"; description="Critical priority task"},
    @{name="priority: high"; color="ff9800"; description="High priority task"},
    @{name="priority: medium"; color="ffeb3b"; description="Medium priority task"},
    @{name="priority: low"; color="4caf50"; description="Low priority task"},
    @{name="phase: 0"; color="0052cc"; description="Phase 0: Prerequisites"},
    @{name="phase: 1"; color="0052cc"; description="Phase 1: Project Scaffolding"},
    @{name="phase: 2"; color="0052cc"; description="Phase 2: Muflone Integration"},
    @{name="phase: 3"; color="0052cc"; description="Phase 3: Marketing Module"},
    @{name="phase: 4"; color="0052cc"; description="Phase 4: Production & Delivery"},
    @{name="phase: 5"; color="0052cc"; description="Phase 5: Cross-Module Integration"},
    @{name="phase: 6"; color="0052cc"; description="Phase 6: Architectural Tests"},
    @{name="phase: 7"; color="0052cc"; description="Phase 7: Documentation & Polish"},
    @{name="status: blocked"; color="6a737d"; description="Blocked by dependencies"}
)

foreach ($label in $labels) {
    $existing = gh label list --repo "$Owner/$Repo" --json name --jq ".[] | select(.name == `"$($label.name)`")" 2>$null
    if (-not $existing) {
        Write-Host "  Creating label: $($label.name)" -ForegroundColor Gray
        gh label create "$($label.name)" --color $label.color --description $label.description --repo "$Owner/$Repo" | Out-Null
    }
}

Write-Host "✓ Labels created" -ForegroundColor Green

Write-Host "`n=== Creating Issues ===" -ForegroundColor Cyan

$issueMap = @{}
$totalTasks = $tasks.Count
$currentTask = 0

foreach ($task in $tasks) {
    $currentTask++
    $taskId = $task.'Task ID'
    $title = "[$taskId] $($task.Title)"
    
    Write-Host "  [$currentTask/$totalTasks] Creating: $taskId" -ForegroundColor Gray
    
    # Build issue body
    $bodyLines = @(
        "**Phase:** $($task.Phase)",
        "**Priority:** $($task.Priority)",
        "**Estimated Time:** $($task.'Estimated Time')",
        "**Assignee:** $($task.Assignee)",
        "**Status:** $($task.Status)"
    )
    
    if ($task.Dependencies) {
        $bodyLines += "**Dependencies:** $($task.Dependencies)"
    }
    
    $bodyLines += ""
    $bodyLines += "---"
    $bodyLines += ""
    $bodyLines += "This issue was automatically created from santa-api-tasks.csv"
    $bodyLines += "Refer to .speckit/tasks/santa-api-tasks.md for detailed acceptance criteria"
    
    $body = $bodyLines -join "`n"

    # Determine labels
    $issueLabels = @()
    $issueLabels += Get-PriorityLabel $task.Priority
    
    # Add phase label
    if ($task.Phase -match 'Phase (\d+)') {
        $phaseNum = $matches[1]
        $issueLabels += "phase: $phaseNum"
    }
    
    # Add blocked label
    if ($task.Status -eq 'Blocked') {
        $issueLabels += 'status: blocked'
    }
    
    $labelArgs = ($issueLabels | ForEach-Object { "--label `"$_`"" }) -join " "
    
    # Create issue
    try {
        $issueUrl = gh issue create `
            --repo "$Owner/$Repo" `
            --title $title `
            --body $body `
            $labelArgs.Split(" ")
        
        # Extract issue number from URL
        $issueNumber = ($issueUrl -split '/')[-1]
        $issueMap[$taskId] = $issueNumber
        
    } catch {
        Write-Warning "Failed to create issue for $taskId : $_"
    }
}

Write-Host "✓ Created $($issueMap.Count) issues" -ForegroundColor Green

Write-Host "`n=== Creating GitHub Project ===" -ForegroundColor Cyan

# Create project (Projects V2)
try {
    $projectData = gh project create `
        --owner $Owner `
        --title $ProjectName `
        --format json | ConvertFrom-Json
    
    $projectNumber = $projectData.number
    Write-Host "✓ Created project #$projectNumber : $ProjectName" -ForegroundColor Green
    
    Write-Host "`n=== Adding Issues to Project ===" -ForegroundColor Cyan
    
    foreach ($taskId in $issueMap.Keys) {
        $issueNumber = $issueMap[$taskId]
        Write-Host "  Adding issue #$issueNumber to project..." -ForegroundColor Gray
        
        try {
            gh project item-add $projectNumber `
                --owner $Owner `
                --url "https://github.com/$Owner/$Repo/issues/$issueNumber" | Out-Null
        } catch {
            Write-Warning "Failed to add issue #$issueNumber to project: $_"
        }
    }
    
    Write-Host "✓ Added issues to project" -ForegroundColor Green
    
} catch {
    Write-Warning "Failed to create project or add issues: $_"
    Write-Host "You can manually create a project and add the issues" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
Write-Host "Created $($issueMap.Count) issues" -ForegroundColor Green
Write-Host "Created project: $ProjectName" -ForegroundColor Green
Write-Host ""
Write-Host "View your project at:" -ForegroundColor Cyan
Write-Host "https://github.com/$Owner/$Repo/projects/$projectNumber" -ForegroundColor White
Write-Host ""
Write-Host "View issues at:" -ForegroundColor Cyan
Write-Host "https://github.com/$Owner/$Repo/issues" -ForegroundColor White
Write-Host ""
Write-Host "Done!" -ForegroundColor Green
