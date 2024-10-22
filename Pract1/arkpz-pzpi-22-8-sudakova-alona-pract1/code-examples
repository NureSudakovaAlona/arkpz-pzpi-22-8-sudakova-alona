//рекомендація 1 - використовувати регістр camelCase для назв змінних, функцій, членів класів та інтерфейсів, методів та їх параметрів
// Поганий приклад
let StudentGrades;
function AddStudentGrade(student, grade) {
  student.gradeList.push(grade);
}

class Tutor {
  StudentList: Array<any>;

  AddNewStudent(student) {
    this.StudentList.push(student);
  }
}

// Гарний приклад
let studentGrades: Map<number, number[]> = new Map();
function addStudentGrade(studentId: number, grade: number): void {
  const grades = studentGrades.get(studentId) || [];
  grades.push(grade);
  studentGrades.set(studentId, grades);
}

class Tutor {
  studentList: Array<number> = [];

  addNewStudent(studentId: number): void {
    if (!this.studentList.includes(studentId)) {
      this.studentList.push(studentId);
      console.log(`Студент ${studentId} доданий.`);
    } else {
      console.log(`Студент з таким ідентифікатором ${studentId} вже доданий.`);
    }
  }
}


//рекомендація 2 - використовувати регістр PascalCase для імен класів та інтерфейсів, перерахувань (enum).
// Поганий приклад
class tutorManagement { 
  addTutor(name, subject) {
    console.log(`Репетитор ${name} доданий до дисципліни ${subject}`);
  }
}

enum status {
  default = 0,
  active = 1,
  banned = 2
}

// Гарний приклад
class TutorManagement { 
  private tutors: { [id: number]: { name: string, subject: string } } = {};

  addTutor(id: number, name: string, subject: string): void {
    if (!this.tutors[id]) {
      this.tutors[id] = { name, subject };
      console.log(`Репетитор ${name} доданий до дисципліни ${subject}`);
    } else {
      console.log(`Репетитор з ідентифікатором ${id} вже існує.`);
    }
  }

  removeTutor(id: number): void {
    if (this.tutors[id]) {
      delete this.tutors[id];
      console.log(`Репетитора з ідентифікатором ${id} видалено.`);
    } else {
      console.log(`Репетитор з ідентифікатором ${id} не існує в системі.`);
    }
  }
}

enum Status {
  Default = 0,
  Active = 1,
  Banned = 2
}


//рекомендація 3 - використовувати оператор let замість var
// Поганий приклад
var currentUserId = 1;
var activeTutors = [];

function addTutor(tutorId) {
  var currentUserId = tutorId;
  activeTutors.push(currentUserId);
}

console.log(currentUserId); // 1

// Гарний приклад
let currentUserId = 1;
let activeTutors: number[] = [];

function addTutor(tutorId: number): void {
  let currentUserId = tutorId;
  activeTutors.push(currentUserId);
  console.log(`Репетитора з ідентифікатором ${tutorId} додано.`);
}

console.log(currentUserId); // 1


//рекомендація 4 - використовувати типізацію
// Поганий приклад
let tutorList = [];

function assignTutorToStudent(studentId, tutorId) {
  tutorList.push({ studentId, tutorId });
}

// Гарний приклад
let tutorList: { studentId: number, tutorId: number }[] = [];

function assignTutorToStudent(studentId: number, tutorId: number): void {
  tutorList.push({ studentId, tutorId });
  console.log(`Репетитора з ідентифікатором ${tutorId} закріплено за студентом з ідентифікатором ${studentId}`);
}

assignTutorToStudent(1, 101);


//рекомендація 5 - не додавати непотрібний контекст
// Поганий приклад
type Student = {
  studentFirstName: string;
  studentLastName: string;
  studentAge: number;
}

function getStudentInfo(student: Student): string {
  return `${student.studentFirstName} ${student.studentLastName}, Age: ${student.studentAge}`;
}

// Гарний приклад
type Student = {
  firstName: string;
  lastName: string;
  age: number;
}

function getStudentInfo(student: Student): string {
  return `${student.firstName} ${student.lastName}, Age: ${student.age}`;
}

const student: Student = { firstName: "Anna", lastName: "Smith", age: 21 };
console.log(getStudentInfo(student)); // Anna Smith, Age: 21


//рекомендація 6 - уникати негативних умовних речень
// Поганий приклад
function isNotTutorAssigned(studentId: number): boolean {
  return !tutorList.some(tutor => tutor.studentId === studentId);
}

if (!isNotTutorAssigned(1)) {
  console.log("За студентом закріплено репетитора");
} else {
  console.log("Репетитор не був закріплений за студентом");
}

// Гарний приклад
function isTutorAssigned(studentId: number): boolean {
  return tutorList.some(tutor => tutor.studentId === studentId);
}

if (isTutorAssigned(1)) {
  console.log("За студентом закріплено репетитора");
} else {
  console.log("Репетитор не був закріплений за студентом");
}


//рекомендація 7 - використовувати принцип єдиної відповідальності (single-responsibility principle)
// Поганий приклад
class Student {
  public createStudentAccount() {
    console.log("Акаунт створено!");
  }

  public calculateStudentGrade() {
    console.log("Оцінку розраховано!");
  }

  public generateStudentReport() {
    console.log("Звіт підготовлено!");
  }
}

// Гарний приклад
class StudentAccount {
  public createStudentAccount(studentId: number): void {
    console.log(`Акаунт для студента з ідентифікатором ${studentId} створено.`);
  }
}

class StudentGrade {
  public calculateStudentGrade(studentId: number, grades: number[]): number {
    const total = grades.reduce((acc, grade) => acc + grade, 0);
    const average = total / grades.length;
    console.log(`Середня оцінка студента з ідентифікатором ${studentId} дорівнює ${average}.`);
    return average;
  }
}

class StudentReport {
  public generateStudentReport(studentId: number): void {
    console.log(`Звіт для студента з ідентифікатором ${studentId} створено.`);
  }
}

//рекомендація 8 - уникати тип «any», коли це можливо
// Поганий приклад
let tutorData: any = { name: "John", subject: "Math" };
tutorData = 123; // Не видасть помилку

function displayTutorInfo(tutor: any) {
  console.log(tutor);
}

// Гарний приклад
type Tutor = {
  name: string;
  subject: string;
};

let tutorData: Tutor = { name: "John", subject: "Math" };
// tutorData = 123; // Це призведе до помилки

function displayTutorInfo(tutor: Tutor): void {
  console.log(`Репетитор: ${tutor.name}, Дисципліна: ${tutor.subject}`);
}

displayTutorInfo(tutorData);
