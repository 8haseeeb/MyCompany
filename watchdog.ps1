$services = @(
    @{ Name = "Promotions API"; Url = "http://localhost:5137/api/health" },
    @{ Name = "SSO API"; Url = "http://localhost:5253/api/health" },
    @{ Name = "API Gateway"; Url = "http://localhost:5089/api/gateway/health" }
)
$intervalSeconds = 5
$statuses = @{}

foreach ($service in $services) {
    $statuses[$service.Name] = "UP"
}

Write-Host "--- Multi-Service Watchdog started ---" -ForegroundColor Cyan
Write-Host "Monitoring:"
foreach ($service in $services) {
    Write-Host " - $($service.Name): $($service.Url)"
}
Write-Host "Checking every $intervalSeconds seconds..."
Write-Host ""

while ($true) {
    foreach ($service in $services) {
        $name = $service.Name
        $url = $service.Url
        
        try {
            # Special case for Gateway Health which returns 503 if downstream is down
            $response = Invoke-WebRequest -Uri $url -Method Get -TimeoutSec 3 -ErrorAction Stop
            
            if ($statuses[$name] -eq "DOWN") {
                Write-Host ""
                Write-Host "******************************************" -ForegroundColor Green
                Write-Host "SERVICE RECOVERED: $name" -ForegroundColor Green
                Write-Host "******************************************" -ForegroundColor Green
                Write-Host "Time:   $(([DateTime]::Now).ToString('yyyy-MM-dd HH:mm:ss'))"
                Write-Host "Status: Back Online"
                Write-Host "******************************************" -ForegroundColor Green
                $statuses[$name] = "UP"
            }
        }
        catch {
            # Capture specific status code for Gateway (it might be 503 if downstream is dead)
            $statusCode = 0
            if ($null -ne $_.Exception.Response) {
                $statusCode = [int]$_.Exception.Response.StatusCode
            }

            if ($statuses[$name] -eq "UP") {
                $color = "Red"
                if ($statusCode -eq 503 -and $name -eq "API Gateway") {
                    $msg = "DOWNSTREAM SERVICE FAILURE"
                } else {
                    $msg = "SERVICE DOWN"
                }

                Write-Host ""
                Write-Host "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" -ForegroundColor $color
                Write-Host "$msg ALERT: $name" -ForegroundColor $color
                Write-Host "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" -ForegroundColor $color
                Write-Host "Time:   $(([DateTime]::Now).ToString('yyyy-MM-dd HH:mm:ss'))"
                Write-Host "Status: Down/Unresponsive"
                Write-Host "URL:    $url"
                Write-Host "Error:  $($_.Exception.Message)"
                if ($statusCode -gt 0) { Write-Host "Code:   $statusCode" }
                Write-Host "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" -ForegroundColor $color
                $statuses[$name] = "DOWN"
            }
        }
    }
    
    # Simple heartbeat indicator
    Write-Host "." -NoNewline
    Start-Sleep -Seconds $intervalSeconds
}
