package main

import (
	"fmt"
	"strings"
)

// TestUTF8Sanitization tests the UTF-8 sanitization function
func TestUTF8Sanitization() {
	fmt.Println("=== Testing UTF-8 Sanitization ===")

	// Test cases with problematic characters
	testCases := []struct {
		name     string
		input    string
		expected string
	}{
		{
			name:     "String with null bytes",
			input:    "IDNIC-PAAS-AS-ID\x00 PT. Awan Kilat\x00 Semesta, ID",
			expected: "IDNIC-PAAS-AS-ID PT. Awan Kilat Semesta, ID",
		},
		{
			name:     "String with control characters",
			input:    "Test\x01\x02\x03String\x1F",
			expected: "TestString",
		},
		{
			name:     "String with valid control characters (tab, newline, CR)",
			input:    "Line1\tTabbed\nLine2\rCarriageReturn",
			expected: "Line1\tTabbed\nLine2\rCarriageReturn",
		},
		{
			name:     "Normal string",
			input:    "Normal ASN Description",
			expected: "Normal ASN Description",
		},
		{
			name:     "Empty string",
			input:    "",
			expected: "",
		},
		{
			name:     "String with only null bytes",
			input:    "\x00\x00\x00",
			expected: "",
		},
	}

	for _, tc := range testCases {
		fmt.Printf("\nTest: %s\n", tc.name)
		fmt.Printf("Input: %q\n", tc.input)

		result := sanitizeUTF8String(tc.input)
		fmt.Printf("Output: %q\n", result)

		if result == tc.expected {
			fmt.Printf("✅ PASS\n")
		} else {
			fmt.Printf("❌ FAIL - Expected: %q\n", tc.expected)
		}
	}

	fmt.Println("\n=== UTF-8 Sanitization Test Complete ===")
}

// TestNormalizationWithSanitization tests normalization functions with problematic input
func TestNormalizationWithSanitization() {
	fmt.Println("\n=== Testing Normalization with Sanitization ===")

	// Create a mock PostgreSQL client for testing
	mockClient := &PostgreSQLClient{}

	// Test ASN info normalization with null bytes
	problematicASNInfo := "IDNIC-PAAS-AS-ID\x00 PT. Awan Kilat\x00 Semesta, ID"
	normalizedASNInfo := mockClient.normalizeASNInfo(problematicASNInfo)
	fmt.Printf("ASN Info with null bytes:\n")
	fmt.Printf("  Input: %q\n", problematicASNInfo)
	fmt.Printf("  Output: %q\n", normalizedASNInfo)

	if !strings.Contains(normalizedASNInfo, "\x00") {
		fmt.Printf("  ✅ Null bytes removed successfully\n")
	} else {
		fmt.Printf("  ❌ Null bytes still present\n")
	}

	// Test category normalization with control characters
	problematicCategory := "bot\x01\x02\x03malware"
	normalizedCategory := mockClient.normalizeCategory(problematicCategory)
	fmt.Printf("\nCategory with control characters:\n")
	fmt.Printf("  Input: %q\n", problematicCategory)
	fmt.Printf("  Output: %q\n", normalizedCategory)

	// Test protocol normalization
	problematicProtocol := "TCP\x00\x01"
	normalizedProtocol := mockClient.normalizeProtocol(problematicProtocol)
	fmt.Printf("\nProtocol with null bytes:\n")
	fmt.Printf("  Input: %q\n", problematicProtocol)
	fmt.Printf("  Output: %q\n", normalizedProtocol)

	// Test malware family normalization
	problematicFamily := "xorddos\x00family"
	normalizedFamily := mockClient.normalizeMalwareFamily(problematicFamily)
	fmt.Printf("\nMalware family with null bytes:\n")
	fmt.Printf("  Input: %q\n", problematicFamily)
	fmt.Printf("  Output: %q\n", normalizedFamily)

	fmt.Println("\n=== Normalization with Sanitization Test Complete ===")
}
