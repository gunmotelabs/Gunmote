# Nuevo repositorio

Bienvenido al nuevo repositorio oficial de Gunmote.

El repositorio original dejó de estar disponible después de que solicitara su cierre como medida preventiva tras detectarse un compromiso de seguridad en mi cuenta de GitHub. Aunque el código del proyecto nunca se vio comprometido, preferí comenzar desde una cuenta completamente nueva para garantizar un entorno de desarrollo seguro y de confianza.

Este repositorio alojará la última versión de Gunmote 1, incluyendo las correcciones y mejoras finales de la rama actual.

En los próximos meses anunciaré la nueva web oficial del proyecto, que centralizará las descargas, documentación, recursos adicionales y todas las novedades relacionadas con Gunmote y otros proyectos.

---

## Gunmote 1.1.1.0

La versión 1.1.1.0 representa la actualización final de Gunmote 1 e incorpora las siguientes mejoras:

- Migración completa a .NET 10, modernizando la base del proyecto y mejorando el rendimiento y la compatibilidad con las versiones actuales de Windows.
- Corregido un problema en el modo Diamond por el que el cálculo del área activa en pantallas 4:3 no se aplicaba correctamente, provocando un posicionamiento incorrecto del puntero.
- Mejoras en la estabilidad general, eliminando diversos bloqueos y cierres producidos durante la comunicación con los Wiimotes y en situaciones de desconexión.
- Correcciones en el procesamiento del cursor y del posicionamiento IR, mejorando la precisión en determinadas configuraciones de pantalla.
- Ampliación de ArcadeOutput, incorporando nuevos eventos y salidas para mejorar la compatibilidad con sistemas de iluminación, recoil y otros dispositivos externos utilizados por MAMEHooker y proyectos similares.
- Compatibilidad ampliada de ArcadeOutput con LampStart, permitiendo reconocer tanto "LampStart" como "LmpStart" para mantener la compatibilidad con configuraciones procedentes de DemulShooter y otros frontends.
- Corregida la detección del aspect ratio en determinadas configuraciones, asegurando que los perfiles 4:3 se apliquen correctamente cuando corresponde.
- Diversas optimizaciones internas y correcciones menores destinadas a mejorar la fiabilidad general de Gunmote.

---

## El futuro: Gunmote 2

Mientras esta versión permanecerá disponible como la última edición de Gunmote 1, el desarrollo ya está centrado en Gunmote 2.

Gunmote 2 será una build desarrollada completamente desde cero, aprovechando toda la experiencia adquirida durante el desarrollo de Gunmote 1 para diseñar una arquitectura más moderna, mantenible y preparada para futuras ampliaciones.

La nueva generación del proyecto incorporará una interfaz completamente rediseñada, una base de código renovada y nuevas funcionalidades orientadas a ofrecer una experiencia de configuración más sencilla, flexible y potente.

A medida que avance el desarrollo se irá publicando más información a través de la futura web oficial del proyecto.

Gracias a todos los usuarios que habéis apoyado Gunmote durante todos estos años con vuestras pruebas, sugerencias y reportes de errores. Vuestra colaboración ha sido fundamental para hacer crecer el proyecto.

# Código fuente

Gunmote es un proyecto distribuido bajo la licencia GNU General Public License (GPL) y, como tal, su código fuente estará disponible en este repositorio.

Lamentablemente, como consecuencia del incidente de seguridad que provocó el cierre del repositorio y de mi cuenta anterior de GitHub, actualmente me encuentro en proceso de recuperar una copia del código fuente. Es posible que la versión recuperada no corresponda exactamente a la versión publicada en este nuevo repositorio, ya que parte del historial de desarrollo se perdió como consecuencia del incidente.

En cuanto disponga de una versión recuperada y verificada, será publicada en este repositorio para cumplir con los términos de la licencia GPL y permitir que la comunidad continúe colaborando con el proyecto.


# New Repository

Welcome to the new official Gunmote repository.

The original repository is no longer available because I requested its closure as a precautionary measure after my GitHub account was compromised. Although the project itself was never affected, I decided to start from a completely new account to ensure a clean and trustworthy development environment.

This repository will host the final release of Gunmote 1, including the latest fixes and improvements for the current generation of the project.

In the coming months, I will announce the new official Gunmote website, which will become the central place for downloads, documentation, additional resources, and future Gunmote project updates and other projects.

---

## Gunmote 1.1.1.0

Version 1.1.1.0 is the final update of Gunmote 1 and includes the following improvements:

- Complete migration to .NET 10, modernizing the project's foundation while improving performance and compatibility with current versions of Windows.
- Fixed an issue in Diamond mode where the active area calculation was not correctly applied on 4:3 displays, resulting in inaccurate pointer positioning.
- Improved overall stability, eliminating several crashes and freezes that could occur during Wii Remote communication and disconnection scenarios.
- Refined cursor processing and IR positioning, improving accuracy across specific display configurations.
- Expanded ArcadeOutput, adding new events and outputs to improve compatibility with lighting systems, recoil devices, and other external hardware used by MAMEHooker and similar projects.
- Extended ArcadeOutput compatibility with LampStart, allowing both "LampStart" and "LmpStart" to be recognized, ensuring compatibility with configurations generated by DemulShooter and other frontends.
- Fixed aspect ratio detection in certain configurations, ensuring that 4:3 profiles are applied correctly whenever appropriate.
- Various internal optimizations and minor fixes to improve Gunmote's overall reliability.

---

## Looking Ahead: Gunmote 2

While this repository will remain available as the home of the final Gunmote 1 release, development is now focused on Gunmote 2.

Gunmote 2 will be a completely rebuilt application developed from the ground up, taking everything learned from Gunmote 1 to create a more modern, maintainable architecture designed for future expansion.

The next generation of the project will feature a completely redesigned user interface, a renewed codebase, and new functionality aimed at providing a simpler, more flexible, and more powerful user experience.

More information will be shared as development progresses through the upcoming official project website.

Thank you to everyone who has supported Gunmote over the years by testing, reporting bugs, and sharing feedback. Your contributions have been essential to the project's growth.


# Source Code

Gunmote is released under the GNU General Public License (GPL), and its source code will be made available through this repository.

Unfortunately, due to the security incident that led to the closure of my previous GitHub account and repository, I am currently working on recovering a copy of the source code. The recovered source code may not exactly match the version published in this new repository, as part of the development history was lost as a result of the security incident.

As soon as I have a recovered and verified version, it will be published in this repository in accordance with the GPL license, allowing the community to continue contributing to the project.
