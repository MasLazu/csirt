package main

import (
	"fmt"
	"strings"
	"unicode"
	"unicode/utf8"
)

// aggressiveSanitizeUTF8String provides more aggressive sanitization for problematic strings
func aggressiveSanitizeUTF8String(s string) string {
	if s == "" {
		return s
	}

	// First pass: remove null bytes and control characters
	var cleaned strings.Builder
	for _, r := range s {
		// Skip null bytes and problematic control characters
		if r == 0 || (r < 32 && r != 9 && r != 10 && r != 13) {
			continue
		}
		// Skip non-printable characters except whitespace
		if !unicode.IsPrint(r) && !unicode.IsSpace(r) {
			continue
		}
		cleaned.WriteRune(r)
	}

	result := cleaned.String()

	// Second pass: ensure valid UTF-8
	if !utf8.ValidString(result) {
		result = strings.ToValidUTF8(result, "")
	}

	// Third pass: remove any remaining null bytes that might have been introduced
	result = strings.ReplaceAll(result, "\x00", "")

	return result
}

// TestAggressiveSanitization tests the more aggressive sanitization approach
func TestAggressiveSanitization() {
	fmt.Println("=== Testing Aggressive UTF-8 Sanitization ===")

	testCases := []struct {
		name  string
		input string
	}{
		{
			name:  "String with embedded null bytes",
			input: "IDNIC\x00PAAS\x00AS\x00ID",
		},
		{
			name:  "String with multiple control characters",
			input: "Test\x01\x02\x03\x04\x05String\x1F\x7F",
		},
		{
			name:  "String with non-printable Unicode",
			input: "Test\u0000\u0001\u0002String",
		},
		{
			name:  "Mixed problematic characters",
			input: "bot\x00\x01malware\x02\x03category",
		},
		{
			name:  "Category with null bytes",
			input: "bot\x00",
		},
		{
			name:  "ASN info with control chars",
			input: "IDNIC-PAAS-AS-ID\x00\x01\x02 PT. Awan Kilat Semesta, ID",
		},
	}

	for _, tc := range testCases {
		fmt.Printf("\nTest: %s\n", tc.name)
		fmt.Printf("Input: %q\n", tc.input)

		// Test current sanitization
		currentResult := sanitizeUTF8String(tc.input)
		fmt.Printf("Current: %q\n", currentResult)

		// Test aggressive sanitization
		aggressiveResult := aggressiveSanitizeUTF8String(tc.input)
		fmt.Printf("Aggressive: %q\n", aggressiveResult)

		// Check for null bytes
		if strings.Contains(currentResult, "\x00") {
			fmt.Printf("❌ Current still has null bytes\n")
		} else {
			fmt.Printf("✅ Current clean\n")
		}

		if strings.Contains(aggressiveResult, "\x00") {
			fmt.Printf("❌ Aggressive still has null bytes\n")
		} else {
			fmt.Printf("✅ Aggressive clean\n")
		}
	}

	fmt.Println("\n=== Aggressive Sanitization Test Complete ===")
}
