# Changelog
## [2.0.0] - 2026-02-19
### Changes
- Added new StreamCopyingResultHandler, which will be invoked on IResult<StreamResponse> types during content negotiation and subsequent response writing.
  The purpose of this is that, so long as facade service methods or similar return an IResult<StreamResponse> result, the service code can just call response.Negotiate(result) in the same way that it does for any other IResult types.
  And then content negotiation will do the rest, i.e. set the response status code and headers, and copy the stream of data to the response body.
- Renamed ResultHandler to SerializingResultHanlder for clarity since the previous name implied that it might be suitable for more general cases where serializing is not required.
  (a class rename is technically a breaking change hence the major version bump, although we tend to extend the JsonResulHandler, which I've not renamed, so the impact should be minimal))
## [1.4.0] - 2026-02-10
### Changes
- Internal refactor: replace visitor pattern with a simpler ResultResponseWriter class
## [1.1.0] - 2025-09-03
### Changes
- Add support for handling arbitrary object response body for 401 Unauthorized responses to the ResultVisitor
## [1.0.0] - 2025-08-11
### Changes
- update packages, update to dotnet 9.0
