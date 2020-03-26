# Referenced from https://docs.microsoft.com/en-us/archive/blogs/jasonn/downloading-files-from-the-internet-in-powershell-with-progress
function DownloadFile($url, $targetFile) {
	try {
		$uri = New-Object "System.Uri" "$url"
		$request = [System.Net.HttpWebRequest]::Create($uri)
		$request.set_Timeout(15000) #15 second timeout
		$response = $request.GetResponse()
		$totalLength = [System.Math]::Floor($response.get_ContentLength() / 1024)
		$responseStream = $response.GetResponseStream()
		$targetStream = New-Object -TypeName System.IO.FileStream -ArgumentList $targetFile, Create
		$buffer = new-object byte[] 10KB
		$count = $responseStream.Read($buffer, 0, $buffer.length)
		$downloadedBytes = $count
		while ($count -gt 0) {
			[System.Console]::CursorLeft = 0
			[System.Console]::Write("Downloaded {0}K of {1}K", [System.Math]::Floor($downloadedBytes / 1024), $totalLength)
			$targetStream.Write($buffer, 0, $count)
			$count = $responseStream.Read($buffer, 0, $buffer.length)
			$downloadedBytes = $downloadedBytes + $count
		}
		[System.Console]::WriteLine()
		$targetStream.Flush()
		$targetStream.Close()
		$targetStream.Dispose()
		$responseStream.Dispose()

		return
	}
	catch {
		Write-Host "Error: " -NoNewline -ForegroundColor Red
		Write-Host "Could not download File"
		Remove-Item $targetFile

		return
	}
}