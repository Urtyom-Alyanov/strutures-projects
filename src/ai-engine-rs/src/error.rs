use std::fmt;

#[repr(u8)]
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum Status {
    Ok = 0,
    NullPointer = 1,
    InvalidArgument = 2,
    DimensionMismatch = 3,
    InvalidStructure = 4,
    Panic = 100,
}

#[derive(Debug)]
pub struct SparseError {
    pub status: Status,
    pub message: String,
}

impl SparseError {
    pub fn new(status: Status, message: impl Into<String>) -> Self {
        Self {
            status,
            message: message.into(),
        }
    }
}

impl fmt::Display for SparseError {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{:?}: {}", self.status, self.message)
    }
}
impl std::error::Error for SparseError {}

pub type Result<T> = std::result::Result<T, SparseError>;

pub(crate) fn fail<T>(status: Status, msg: impl Into<String>) -> Result<T> {
    Err(SparseError::new(status, msg))
}

pub(crate) fn check_len(name: &str, expected: usize, actual: usize) -> Result<()> {
    if expected != actual {
        return fail(
            Status::DimensionMismatch,
            format!("{name}: expected length {expected}, got {actual}"),
        );
    }
    Ok(())
}
